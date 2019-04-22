using Android.App;
using System;

namespace SpotyPie.Models
{
    public class SongUpdate
    {
        private object Lock { get; set; }

        private int SongId { get; set; }
        private float PlayedProcent { get; set; }

        private int Seconds { get; set; }

        public bool IsUpdated { get; set; }

        public SongUpdate(int? id)
        {
            if (id == null)
                return;
            this.Lock = new object();
            this.SongId = (int)id;
            this.PlayedProcent = 0;
            this.IsUpdated = false;
        }

        public void CalculateTime(long SongDuration, Action action)
        {
            lock (Lock)
            {
                if (!IsUpdated)
                {
                    this.Seconds++;
                    //PlayedProcent = (SongDuration / (Seconds * 100000));

                    if (Seconds > 60)
                    {
                        action.Invoke();
                        IsUpdated = true;
                    }
                }
            }
        }
    }
}