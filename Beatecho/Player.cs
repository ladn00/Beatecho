using Beatecho.DAL;
using Beatecho.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Beatecho
{
    public class Player
    {
        public List<Song> Queue { get; set; }
        public MediaElement MediaElement { get; set; }
        public Song CurrentSong { get; set; }
        public int Index { get; set; } = 0;
        public bool IsPlaying { get; set; } = false;
        public object AlbumPlaylist { get; set; }
        public StackPanel CurrentSongBar { get; set; }
        public Slider TrackSlider { get; set; }
        private bool isDragging = false;
        private DispatcherTimer Timer { get; set; }

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
            Index = 0;
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

            if (Queue[Index + 1] != null)
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
