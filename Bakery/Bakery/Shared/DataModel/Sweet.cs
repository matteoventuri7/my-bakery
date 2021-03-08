using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bakery.Shared.DataModel
{
    public class Sweet
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime? StartSellingDate { get; set; }
        public int Quantity { get; set; }
        public string ImageFileName { get; set; }
        public List<RecipeIngredient> Recipe { get; set; } = new List<RecipeIngredient>();

        public double CalculateDayPrice()
        {
            double coef = DayOld() switch
            {
                0 => 1,
                1 => 0.8,
                2 => 0.2,
                _ => 0
            };
            return Price * coef;
        }
        
        public int DayOld()
        {
            var delta = DateTime.Now - StartSellingDate.GetValueOrDefault(DateTime.Now);
            return delta.Days;
        }

        public bool IsEatable()
        {
            return (DateTime.Now - StartSellingDate).GetValueOrDefault().Days < 3;
        }

        public bool IsSellable()
        {
            return StartSellingDate is null;
        }

        public void StartSelling()
        {
            if (IsSellable())
                StartSellingDate = DateTime.Now;
            else throw new ApplicationException("Product is not sellable.");
        }
    }
}
