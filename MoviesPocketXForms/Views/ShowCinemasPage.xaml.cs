﻿namespace MoviesPocketXForms.Views
{
	using Xamarin.Forms;
    using Xamarin.Forms.Maps;
    using ViewModels;
    using Plugin.Geolocator;
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;

    public partial class ShowCinemasPage : ContentPage
    {
        public Command updateRegion;
        public Geocoder geocoder;

        public ShowCinemasPage()
        {
            InitializeComponent();
            updateRegion = new Command(async () => await moveMapToUserLocationAsync());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
			
            (this.BindingContext as ShowCinemasPageViewModel).Init();
            geocoder = new Geocoder();

            updateRegion.Execute(null);

        }

        public async Task moveMapToUserLocationAsync()
        {
            CrossGeolocator.Current.DesiredAccuracy = 50;
            var position = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(10));
            if (position != null)
            {
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMiles(15)).WithZoom(3));
            }


            var cinemas = await (this.BindingContext as ShowCinemasPageViewModel).CinemasOnArea(position.Longitude, position.Latitude);

            foreach (var cinema in cinemas) {
				var cinemaPosition = new Position(cinema.Geometry.Location.Lat,cinema.Geometry.Location.Lng);
                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = cinemaPosition,
                    Label = cinema.Name,
                    Address = "Puntuación: " + cinema.Rating
                };
                MyMap.Pins.Add(pin);
            }

        }

		public async Task<List<Position>> GetPositionsByAddress(string address)
		{
            var locations = await geocoder.GetPositionsForAddressAsync(address);
            return locations.ToList();
		}
    }


}
