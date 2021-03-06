﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft;
using Newtonsoft.Json;

using Xamarin.Forms;
using Newtonsoft.Json.Linq;

namespace KitchenSupport
{
    public class RecipeStream : ContentPage
    {

        public class Recipes
        {
            public List<recipe> recipes { get; set; }
        }
        public class recipe
        {
            public string recipeName { get; set; }
            public string[] smallImageUrls { get; set; }
            public int id { get; set; }
            public int rating { get; set; }
            public string[] ingredients { get; set; }
            public int? totalTimeInSeconds { get; set; }
        }
        public List<recipe> parseRecipes(string request)
        {
            var result = JsonConvert.DeserializeObject<Recipes>(request);
            return result.recipes;
        }

        public class Ingredient
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public string unit { get; set; }

            public string unitAndQuantity
            {
                get
                {
                    return quantity + " " + unit;
                }
            }
        }

        public class RecipeDetails : ContentPage
        {
            public static ListView listview;
            public static List<Ingredient> ingredients;
            public RecipeDetails(recipe r)
            {
                Label header = new Label
                {
                    Text = r.recipeName,
                    Font = Font.BoldSystemFontOfSize(40),
                    HorizontalOptions = LayoutOptions.Center
                };
                var recipePic = new Image
                {
                    Aspect = Aspect.AspectFill,
                    HeightRequest = 200,
                    WidthRequest = 200
                };

                recipePic.Source = ImageSource.FromUri(new Uri(r.smallImageUrls[0].Substring(0, r.smallImageUrls[0].Length - 4)));


                /*String ratingImageName = r.rating.ToString() + "star.png";
                var ratingImage = new Image { Aspect = Aspect.AspectFit };
                ratingImage.Source = ImageSource.FromFile(ratingImageName);*/

                string[] ratingImageLinks = new string[] { "http://i.imgur.com/7qq8zdR.png", "http://i.imgur.com/BRwowMP.png", "http://i.imgur.com/dNUdKiO.png", "http://i.imgur.com/zK4JmCG.png", "http://i.imgur.com/61WSiZf.png", "http://i.imgur.com/7J7BYuv.png" };
                var ratingImage = new Image { Aspect = Aspect.AspectFit };
                ratingImage.Source = ImageSource.FromUri(new Uri(ratingImageLinks[r.rating]));


                string hourOrHours = "hours";
                string minuteOrMinutes = "minutes";

                int time = 0;
                if (r == null)
                {
                    return;

                }

                Label cookingTimeLabel = new Label();

                if (r.totalTimeInSeconds == null)
                {
                    cookingTimeLabel.Text = "";
                    cookingTimeLabel.Font = Font.BoldSystemFontOfSize(1);
                }
                else
                {
                    time = (int) r.totalTimeInSeconds;

                    int hours = (int)(time / 3600);
                    int leftOverSeconds = time - (hours * 3600);
                    int minutes = (int)(leftOverSeconds / 60);


                    if (hours == 1)
                    {
                        hourOrHours = "hour";
                    }

                    if (minutes == 1)
                    {
                        minuteOrMinutes = "minute";
                    }

                    String cookingTime = "";

                    if (hours != 0)
                    {
                        cookingTime += hours.ToString() + " " + hourOrHours;
                    }

                    if (minutes != 0)
                    {
                        if (hours != 0)
                            cookingTime += ", ";

                        cookingTime += minutes.ToString() + " " + minuteOrMinutes;
                    }

                    cookingTimeLabel.Text = "Time to make: " + cookingTime;
                    cookingTimeLabel.Font = Font.BoldSystemFontOfSize(25);
                }

                Label ingredientLabel = new Label
                {
                    Text = "Ingredients:",
                    Font = Font.BoldSystemFontOfSize(25)
                };

                listview = new ListView();
                listview.RowHeight = 40;

                ingredients = new List<Ingredient>();
                for (int i = 0; i < r.ingredients.Length; i++)
                {
                    ingredients.Add(new Ingredient { name = r.ingredients[i], quantity = 0, unit = " " });
                }
                listview.ItemsSource = ingredients;

                listview.ItemTemplate = new DataTemplate(typeof(TextCell));
                listview.ItemTemplate.SetBinding(TextCell.TextProperty, ".name");
                //listview.ItemTemplate.SetBinding(TextCell.DetailProperty, ".unitAndQuantity");

                Button viewRecipeButton = new Button();
                viewRecipeButton.Text = "View Recipe";
                viewRecipeButton.BackgroundColor = Color.FromHex("77D065");

                viewRecipeButton.Clicked += delegate {
                    Device.OpenUri(new Uri("http://bfy.tw/333q"));
                };

                this.Content = new StackLayout
                {
                    Spacing = 20,
                    Padding = 50,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        header,
                        recipePic,
                        ratingImage,
                        cookingTimeLabel,
                        ingredientLabel,
                        listview,
                        viewRecipeButton
                    }
                };
            }
        }

        public RecipeStream()
        {
            int count = 1;
            var client = new HttpClient();
            string url = "http://api.kitchen.support/stream";
            var response = client.GetStringAsync(new Uri(url));

            if (response == null)
            {
                return;
            }
            var recipes = parseRecipes(response.Result);
            var recipePic = new Image
            {
                Aspect = Aspect.AspectFill,
                HeightRequest = 200,
                WidthRequest = 200
            };
            recipePic.Source = ImageSource.FromUri(new Uri(recipes[0].smallImageUrls[0].Substring(0, recipes[0].smallImageUrls[0].Length - 4)));

            Label header = new Label
            {
                Text = "Your Recipe Stream",
                Font = Font.BoldSystemFontOfSize(30),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start

            };
            Label recipeName = new Label
            {
                Text = recipes[0].recipeName,
                Font = Font.BoldSystemFontOfSize(20),
                HorizontalOptions = LayoutOptions.Center
            };
            Button like = new Button
            {
                Text = "Like",
                BackgroundColor = Color.FromHex("77D065")
            };
            Button dislike = new Button
            {
                Text = "Dislike",
                BackgroundColor = Color.FromHex("77D065")
            };
            Button openRecipe = new Button
            {
                Text = "More Info",
                BackgroundColor = Color.FromHex("77D065")
            };
            like.Clicked += (sender, e) =>
            {
                if (count == 30)
                {
                    count = 0;
                }
                recipePic.Source = ImageSource.FromUri(new Uri(recipes[count].smallImageUrls[0].Substring(0, recipes[count].smallImageUrls[0].Length - 4)));
                recipeName.Text = recipes[count].recipeName;
                count++;
            };
            dislike.Clicked += (sender, e) =>
            {
                if (count == 30)
                {
                    count = 0;
                }
                recipePic.Source = ImageSource.FromUri(new Uri(recipes[count].smallImageUrls[0].Substring(0, recipes[count].smallImageUrls[0].Length - 4)));
                recipeName.Text = recipes[count].recipeName;
                count++;
            };
            var browser = new WebView();
            openRecipe.Clicked += async (sender, e) =>
            {
                await Navigation.PushModalAsync(new RecipeDetails(recipes[count - 1]));
            };
            Content = new StackLayout
            {
                Spacing = 20,
                Padding = 50,
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    header,
                    recipeName,
                    recipePic,
                    like,
                    dislike,
                    openRecipe
                }
            };
        }
    }
}
