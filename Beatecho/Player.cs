using Beatecho.DAL;
using Beatecho.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Beatecho
{
    public class Player
    {
        public List<Song> Queue { get; set; }
        private MediaElement MediaElement { get; set; }
        public Song CurrentSong { get; set; }
        public int Index { get; set; } = 0;
        public bool IsPlaying { get; set; } = false;

        public Player(MediaElement mediaElement) 
        { 
            this.MediaElement = mediaElement;
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
                MediaElement.Source = new Uri(CurrentSong.Link);
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

            if (Index - 1 > Queue.Count)
                Index--;
            else
                Index = Queue.Count - 1;

            SetSong();
            Play();
        }
    }
}
