﻿using System.Collections.Generic;
using Gupy.Core.Dtos;
using MediatR;

namespace Gupy.Business.Queries.Categories.GetCategories
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>>
    {
        public bool? HasProducts { get; set; }
    }
}