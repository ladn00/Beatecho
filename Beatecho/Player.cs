using Beatecho.DAL.Models;
using System.Windows.Controls;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using System.Linq;

namespace Beatecho
{
    public class Player
    {
        public List<Song> Queue { get; set; }
        public List<Song> UnshuffledQueue { get; set; }
        public MediaElement MediaElement { get; set; }
        public Song CurrentSong { get; set; }
        public int Index { get; set; } = 0;
        public object AlbumPlaylist { get; set; }
        public StackPanel CurrentSongBar { get; set; }
        public Slider TrackSlider { get; set; }
        private bool isDragging = false;
        private DispatcherTimer Timer { get; set; }
        private bool IsCurrentlyPlaying { get; set; }
        public event Action PlayPauseChanged;
        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    PlayPauseChanged?.Invoke();
                }
            }
        }

        public Player(MediaElement mediaElement, StackPanel currentSongBar, Slider trackSlider)
        {
            MediaElement = mediaElement;
            CurrentSongBar = currentSongBar;
            TrackSlider = trackSlider;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(500);
            Timer.Tick += Timer_Tick;
        }

        public void SetQueue(List<Song> newQueue)
        {
            Queue = new List<Song>(newQueue);
            UnshuffledQueue = new List<Song>(newQueue);

            if (IsShuffled)
            {
                ShuffleQueue();
            }
            
            Index = 0;
        }

        public bool IsShuffled { get; set; } = false;

        public void ShuffleQueue()
        {
            IsShuffled = true;
            if (Queue == null || Queue.Count <= 1) return;

            Random rng = new Random();

            var playedSongs = Queue.Take(Index+1).ToList();
            var remainingSongs = Queue.Skip(Index + 1).OrderBy(x => rng.Next()).ToList();

            Queue = playedSongs.Concat(remainingSongs).ToList();
        }

        public void UnshuffleQueue()
        {
            IsShuffled = false;
            if (Queue == null || UnshuffledQueue == null || Queue.Count <= 1) return;

            var playedSongs = Queue.Take(Index + 1).ToList();

            var remainingSongs = UnshuffledQueue.Where(song => !playedSongs.Contains(song)).ToList();

            Queue = playedSongs.Concat(remainingSongs).ToList();

        }

        public void SetSong()
        {
            try
            {
                CurrentSong = Queue[Index];
                MediaElement.Source = new Uri(CurrentSong.Link!);
                if (MediaElement.NaturalDuration.HasTimeSpan)
                {
                    TrackSlider.Maximum = MediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Play()
        {
            try
            {
                MediaElement.Play();
                IsPlaying = true;
                CurrentSongBar.DataContext = CurrentSong;
                Timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Pause()
        {
            try
            {
                MediaElement.Pause();
                IsPlaying = false;
                Timer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Stop()
        {
            try
            {
                MediaElement.Stop();
                IsPlaying = false;
                TrackSlider.Value = 0;
                Timer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PlayNext()
        {
            if (CurrentSong == null)
                return;

            Stop();

            if (Index + 1 < Queue.Count)
                Index++;
            else
                Index = 0;

            SetSong();
            Play();
        }

        public void PlayPrev()
        {
            if (CurrentSong == null)
                return;

            Stop();

            if (Index - 1 >= 0)
                Index--;
            else
                Index = Queue.Count - 1;

            SetSong();
            Play();
        }

        public void ValueOfBarChanged()
        {
            if (MediaElement.NaturalDuration.HasTimeSpan && !isDragging)
            {
                TimeSpan newPosition = TimeSpan.FromSeconds(TrackSlider.Value);
                MediaElement.Position = newPosition;
            }
        }

        public void DragStarted()
        {
            isDragging = true;
            Timer.Stop();
        }

        public void DragCompleted()
        {
            isDragging = false;
            TimeSpan newPosition = TimeSpan.FromSeconds(TrackSlider.Value);
            MediaElement.Position = newPosition;
            Timer.Start();
        }

        public void MediaEnded()
        {
            Stop();

            if (Queue.Count >= Index + 1)
                PlayNext();
        }

        public void VolumeChanged(double volume)
        {
            MediaElement.Volume = volume;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (MediaElement.NaturalDuration.HasTimeSpan && !isDragging)
            {
                TrackSlider.Value = MediaElement.Position.TotalSeconds;
            }
            else if (!MediaElement.NaturalDuration.HasTimeSpan)
            {
                if (MediaElement.NaturalDuration.HasTimeSpan == false && MediaElement.Position.TotalSeconds > 0)
                {
                    TrackSlider.Value = (MediaElement.Position.TotalSeconds / 100) * TrackSlider.Maximum;
                }
            }
        }
    }
}
