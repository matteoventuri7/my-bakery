using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bakery.Client.ViewModel
{
    public class NewRecipeIngredient
    {
        [Required]
        public int? IngredientId { get; set; }
        [Required]
        public double Quantity { get; set; }
    }
}
