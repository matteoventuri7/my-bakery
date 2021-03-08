using Bakery.Client.ViewModel;
using Bakery.Shared.DataModel;
using Bakery.Shared.TransferModel;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Bakery.Client.Pages
{
    public partial class SweetUpsert
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public HttpClient http { get; set; }
        [Inject]
        public SweetAlertService sw { get; set; }

        private Sweet s = new Sweet();
        private NewRecipeIngredient nri = new NewRecipeIngredient();
        private Ingredient[] ingredients;
        private string TitlePage;
        private IBrowserFile fileToUpload;

        private async Task SaveSweet()
        {
            try
            {
                bool success;
                SweetWithImage si = new SweetWithImage(s);
                if (fileToUpload is not null)
                {
                    using Stream stream = fileToUpload.OpenReadStream();
                    using MemoryStream ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    UploadedFile uploadedFile = new UploadedFile(fileToUpload.Name, ms.ToArray());
                    si.ImageFile = uploadedFile;
                }

                if (si.Sweet.Id == default(int))
                {
                    var resp = await http.PostAsJsonAsync("sweet", si);
                    success = resp.IsSuccessStatusCode;
                }
                else
                {
                    var resp = await http.PutAsJsonAsync("sweet/withimage", si);
                    success = resp.IsSuccessStatusCode;
                }

                if (success)
                {
                    s.ImageFileName = fileToUpload.Name;
                    await sw.FireAsync("Success", "Sweet save success.", SweetAlertIcon.Success);
                    TitlePage = "Modify sweet";                    
                }
                else
                {
                    await sw.FireAsync("Error", "Something is wrong.", SweetAlertIcon.Error);
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        private async Task StoreImage(InputFileChangeEventArgs args)
        {
            if (args.FileCount != 1)
            {
                await sw.FireAsync("Errore", "You can upload only one file.", SweetAlertIcon.Error);
            }
            else
            {
                fileToUpload = args.File;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                var loadIngredients = http.GetFromJsonAsync<Ingredient[]>("ingredient");
                if (Id != default(int))
                {
                    s = await http.GetFromJsonAsync<Sweet>($"sweet/{Id}");
                    TitlePage = "Modify sweet";
                }
                else
                {
                    TitlePage = "Insert new sweet";
                }

                ingredients = loadIngredients.Result;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }


        async Task RemoveRecipeItem(RecipeIngredient ri)
        {
            var resp = await http.DeleteAsync($"recipe/{ri.SweetId}/{ri.IngredientId}");
            if (resp.IsSuccessStatusCode)
            {
                s.Recipe.Remove(ri);
                await sw.FireAsync("Success", "Ingredient removed from recipe.", SweetAlertIcon.Success);
            }
            else
            {
                await sw.FireAsync("Error", "Something is wrong.", SweetAlertIcon.Error);
            }
        }

        async Task InsertRecipeIngredient()
        {
            var ri = new RecipeIngredient
            {
                IngredientId = nri.IngredientId.Value,
                Quantity = nri.Quantity,
                SweetId = s.Id
            };

            var resp = await http.PostAsJsonAsync("recipe", ri);
            if (resp.IsSuccessStatusCode)
            {
                await sw.FireAsync("Success", "Ingredient added to recipe.", SweetAlertIcon.Success);
                s = await http.GetFromJsonAsync<Sweet>($"sweet/{Id}");
            }
            else
            {
                await sw.FireAsync("Error", "Something is wrong.", SweetAlertIcon.Error);
            }
        }
    }
}
