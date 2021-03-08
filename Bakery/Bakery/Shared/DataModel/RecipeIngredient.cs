using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bakery.Shared.DataModel
{
    public class RecipeIngredient
    {
        public int SweetId { get; set; }
        public Ingredient Ingredient { get; set; }
        public int IngredientId { get; set; }
        [Required]
        public double Quantity { get; set; }
    }
}
