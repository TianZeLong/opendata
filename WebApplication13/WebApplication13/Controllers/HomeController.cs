﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
<<<<<<< HEAD
=======
    //我添加了一个git ignore文件，这样下次你上传的时候就不会上传没用的文件了
>>>>>>> 2c23940dcdf1f01c26cc09f777dc4337d6d2d046
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult InitialRec(string search )
        {
            string searchstr = null;        
            var gender= Request.Form["option"];
            var age = Request.Form["age"];
            Dictionary<string, string> recipesMap = new Dictionary<string, string>();
            if (search == null)
            {
                 searchstr = "http://food2fork.com/api/search?key=b18c7f80205225f5a6c3585901b06092&q=chicken";
            }else
            {
                 searchstr = "http://food2fork.com/api/search?key=b18c7f80205225f5a6c3585901b06092&q=" + search;              
            }
            var request = (HttpWebRequest)WebRequest.Create(searchstr);
            var response = (HttpWebResponse)request.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            RecipesRootObject rb = JsonConvert.DeserializeObject<RecipesRootObject>(responseString);
            RecipesList recipesList = new RecipesList();
            recipesList.Rlist = new List<SpecificRecipe>();
            int number = 0;
            foreach (Recipes recipes in rb.recipes)
            {  
                if (!recipesMap.ContainsKey(recipes.title))
                {
                    SpecificRecipe specificrecipe = new SpecificRecipe();
                    specificrecipe.title = recipes.title;
                    specificrecipe.ImageUrl = recipes.image_url;
                    specificrecipe.Id = recipes.recipe_id;
                    specificrecipe.Ingredients = GetIngredient(recipes.recipe_id);

                    string[] searchArray = Regex.Split(specificrecipe.Ingredients, ",", RegexOptions.IgnoreCase);
                    specificrecipe.Calorie = getNutrition(searchArray, recipes.recipe_id);
                    recipesList.Rlist.Add(specificrecipe);
                }
                number++;
                if (number == 9)
                {
                    break;
                }
            }
            if (search == null)
            {
               return View("MenuList",recipesList);
            }
            else
            {
               return PartialView("Recipeslist", recipesList);
            }
           
        }
        public string GetIngredient(string recipeid)
        {   string totalIngredient = "";
        
            string searchstr = "http://food2fork.com/api/get?key=b18c7f80205225f5a6c3585901b06092&rId=" + recipeid;
            var request = (HttpWebRequest)WebRequest.Create(searchstr);
            var response = (HttpWebResponse)request.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
      
            SpecificRecipeRootObject rb = JsonConvert.DeserializeObject<SpecificRecipeRootObject>(responseString);
            int i = 0;
            foreach(string ingredient in rb.recipe.ingredients)
            {
                if (i != 0)
                {
                    totalIngredient = totalIngredient + "," + ingredient;
                }
                else
                {
                    totalIngredient = totalIngredient + ingredient;
                }
                i++;
            }
           // string[] searchArray = Regex.Split(totalIngredient, ",", RegexOptions.IgnoreCase);
            return totalIngredient;
        }

        

        public string getNutrition(string[] searchArray, string id)
        {
            String url = "https://trackapi.nutritionix.com/v2/natural/nutrients";
            searchArray[searchArray.Length - 1] = searchArray[searchArray.Length - 1].Replace("\n", "");
            for (int i = 0; i < searchArray.Length; i++)
            {
                if (searchArray[i].Contains("\""))
                {
                    searchArray[i]= searchArray[i].Replace("\"", " ");
                }
                if (searchArray[i].Contains("&"))
                {
                    searchArray[i] = searchArray[i].Replace("&", " ");
                }

            }
            String content = "{\"query\":\"";
            //拼接查询字符串
             for (int i = 0; i < searchArray.Length; i++)
             {
                 content = content + searchArray[i]+ " and ";
                 if (i == searchArray.Length - 1)
                 {
                     content = content + "\"}";
                }
             }
           
            String result = "";
            Dictionary<string, string> calroieMap = new Dictionary<string, string>();
            //发送请求
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers.Add("x-app-id", "7258bd60");
            req.Headers.Add("x-app-key", "d8de75744e7faca5111b0e60efc09549");
            byte[] data = Encoding.UTF8.GetBytes(content);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容  
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            //json解析
            RootObject rb = JsonConvert.DeserializeObject<RootObject>(result);
            foreach (Foods ep in rb.foods)
            {
                if (!calroieMap.ContainsKey(ep.food_name))
                {
                    calroieMap.Add(ep.food_name, ep.nf_calories);
                }
            }
            double calroie = 0;
            foreach(var item in calroieMap)
            {
                calroie = calroie + double.Parse(item.Value);
            }

            return calroie.ToString();
            
        }

        public int GetDefaultRecipe()
        {
            return 1;
        }




        public class Full_nutrients
        {
            public string attr_id { get; set; }
            public string value { get; set; }
        }

        public class Metadata
        {
        }

        public class Tags
        {
            public string item { get; set; }
            public string measure { get; set; }
            public string quantity { get; set; }
            public string tag_id { get; set; }
        }

        public class Alt_measures
        {
            public string serving_weight { get; set; }
            public string measure { get; set; }
            public string seq { get; set; }
            public string qty { get; set; }
        }

        public class Photo
        {
            public string thumb { get; set; }
            public string highres { get; set; }
        }

        public class Foods
        {
            public string food_name { get; set; }
            public string brand_name { get; set; }
            public string serving_qty { get; set; }
            public string serving_unit { get; set; }
            public string serving_weight_grams { get; set; }
            public string nf_calories { get; set; }
            public string nf_total_fat { get; set; }
            public string nf_saturated_fat { get; set; }
            public string nf_cholesterol { get; set; }
            public string nf_sodium { get; set; }
            public string nf_total_carbohydrate { get; set; }
            public string nf_dietary_fiber { get; set; }
            public string nf_sugars { get; set; }
            public string nf_protein { get; set; }
            public string nf_potassium { get; set; }
            public string nf_p { get; set; }
            public List<Full_nutrients> full_nutrients { get; set; }
            public string nix_brand_name { get; set; }
            public string nix_brand_id { get; set; }
            public string nix_item_name { get; set; }
            public string nix_item_id { get; set; }
            public string upc { get; set; }
            public string consumed_at { get; set; }
            public Metadata metadata { get; set; }
            public string source { get; set; }
            public string ndb_no { get; set; }
            public Tags tags { get; set; }
            public List<Alt_measures> alt_measures { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string meal_type { get; set; }
            public Photo photo { get; set; }
        }

        public class RootObject
        {
            public List<Foods> foods { get; set; }
        }

        public class Recipes
        {
            public string publisher { get; set; }
            public string f2f_url { get; set; }
            public string title { get; set; }
            public string source_url { get; set; }
            public string recipe_id { get; set; }
            public string image_url { get; set; }
            public string social_rank { get; set; }
            public string publisher_url { get; set; }
        }

        public class RecipesRootObject
        {
            public string count { get; set; }
            public List<Recipes> recipes { get; set; }
        }

      
        public class Recipe
        {
            public string publisher { get; set; }
            public string f2f_url { get; set; }
            public List<string> ingredients { get; set; }
            public string source_url { get; set; }
            public string recipe_id { get; set; }
            public string image_url { get; set; }
            public string social_rank { get; set; }
            public string publisher_url { get; set; }
            public string title { get; set; }
        }

        public class SpecificRecipeRootObject
        {
            public Recipe recipe { get; set; }
        }

    }
}