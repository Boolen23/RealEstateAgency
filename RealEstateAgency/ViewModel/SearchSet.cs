using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RealEstateAgency.ViewModel
{
    public class SearchSet
    {
        public SearchSet()
        {
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            SearchSetChanged?.Invoke(null, null);
            timer.Stop();
        }
        private void RefreshTimer() { timer.Stop(); timer.Start(); }
        public event EventHandler SearchSetChanged;

        private int? minPrice;
        public int? MinPrice
        {
            get => minPrice;
            set
            {
                if (minPrice != value)
                {
                    minPrice = value;
                    RefreshTimer();
                }
            }
        }
        private int? maxPrice;
        public int? MaxPrice
        {
            get => maxPrice;
            set
            {
                if (maxPrice != value)
                {
                    maxPrice = value;
                    RefreshTimer();
                }
            }
        }

        private int? minArea;
        public int? MinArea
        {
            get => minArea;
            set
            {
                if (minArea != value)
                {
                    minArea = value;
                    RefreshTimer();
                }
            }
        }
        private int? maxArea;
        public int? MaxArea
        {
            get => maxArea;
            set
            {
                if (maxArea != value)
                {
                    maxArea = value;
                    RefreshTimer();
                }
            }
        }

        private int? minFloor;
        public int? MinFloor
        {
            get => minFloor;
            set
            {
                if (minFloor != value)
                {
                    minFloor = value;
                    RefreshTimer();
                }
            }
        }
        private int? maxFloor;
        public int? MaxFloor
        {
            get => maxFloor;
            set
            {
                if (maxFloor != value)
                {
                    maxFloor = value;
                    RefreshTimer();
                }
            }
        }

        private int? minRooms;
        public int? MinRooms
        {
            get => minRooms;
            set
            {
                if (minRooms != value)
                {
                    minRooms = value;
                    RefreshTimer();
                }
            }
        }
        private int? maxRooms;
        public int? MaxRooms
        {
            get => maxRooms;
            set
            {
                if (maxRooms != value)
                {
                    maxRooms = value;
                    RefreshTimer();
                }
            }
        }

        private int? minFloorNumbers;
        public int? MinFloorNumbers
        {
            get => minFloorNumbers;
            set
            {
                if (minFloorNumbers != value)
                {
                    minFloorNumbers = value;
                    RefreshTimer();
                }
            }
        }
        private int? maxFloorNumbers;
        public int? MaxFloorNumbers
        {
            get => maxFloorNumbers;
            set
            {
                if (maxFloorNumbers != value)
                {
                    maxFloorNumbers = value;
                    RefreshTimer();
                }
            }
        }

        DispatcherTimer timer = new DispatcherTimer();
    }
}
