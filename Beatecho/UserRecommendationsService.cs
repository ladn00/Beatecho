using Beatecho.DAL.Models;
using Beatecho.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationContext = Beatecho.DAL.ApplicationContext;
using System.Text.Json;

namespace Beatecho
{
    public class UserRecommendationsService
    {
        private readonly ApplicationContext _context;

        public UserRecommendationsService()
        {
            _context = new ApplicationContext();
        }

        public async Task UpdateRecommendationsAsync(int userId)
        {
            var likedTracks = await _context.FavoriteTracks
                .Where(ul => ul.UserId == userId)
            .Select(ul => ul.SongId)
            .ToListAsync();

            var playlistTracks = await _context.PlaylistSongs
                .Where(pt => _context.PlaylistUsers
                    .Where(up => up.UserId == userId)
                    .Select(up => up.PlaylistId)
                    .Contains(pt.PlaylistId))
                    .Select(pt => pt.SongId)
                    .ToListAsync();

            var allTracks = likedTracks.Concat(playlistTracks).Distinct().ToList();

            var recommendedTracks = GetSimilarTracks(allTracks);

            var recommendation = await _context.UserRecommendations
                                        .AsTracking()
                                        .FirstOrDefaultAsync(ur => ur.UserId == userId);

            string jsonRecommendedTracks = JsonSerializer.Serialize(recommendedTracks);

            try
            {
                if (recommendation != null)
                {
                    recommendation.Recommendations = recommendedTracks;
                    recommendation.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    _context.UserRecommendations.Add(new UserRecommendation
                    {
                        UserId = userId,
                        Recommendations = recommendedTracks,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                var result = await _context.SaveChangesAsync();

            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                if (ex.InnerException != null)
                {
                    MessageBox.Show($"Подробнее: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка: {ex.Message}");
            }
        }

        private List<int> GetSimilarTracks(List<int> allTracks)
        {
            if (!allTracks.Any()) return new List<int>();

            var genreCounts = _context.SongGenres
                .Where(sg => allTracks.Contains(sg.SongId))
                .GroupBy(sg => sg.GenreId)
                .Select(g => new { GenreId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            var topGenres = genreCounts.Select(g => g.GenreId).ToList();

            var similarTracks = _context.SongGenres
                .Where(sg => topGenres.Contains(sg.GenreId) && !allTracks.Contains(sg.SongId))
                .GroupBy(sg => sg.SongId)
                .Select(g => new { SongId = g.Key, MatchCount = g.Count() })
                .OrderByDescending(g => g.MatchCount)
                .Take(20)
                .Select(g => g.SongId)
                .ToList();

            return similarTracks;
        }

        public async Task<List<int>> GetUserRecommendationsAsync(int userId)
        {
            var recommendation = await _context.UserRecommendations.FindAsync(userId);
            return recommendation?.Recommendations ?? new List<int>();
        }
    }
}
