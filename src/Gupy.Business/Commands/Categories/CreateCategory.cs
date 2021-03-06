﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Gupy.Business.Commands.Photos;
using Gupy.Core.Dtos;
using Gupy.Core.Exceptions;
using Gupy.Core.Interfaces.Common;
using Gupy.Core.Interfaces.Data.Repositories;
using Gupy.Domain;
using MediatR;

namespace Gupy.Business.Commands.Categories
{
    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public string Name { get; set; }
        public IFile Photo { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMediator mediator, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (await _categoryRepository.CategoryExists(request.Name))
            {
                throw new NotValidException(nameof(request.Name),
                    $"Category with such name ({request.Name}) already exists");
            }

            var category = new Category {Name = request.Name};
            if (request.Photo != null)
            {
                var fileName = await _mediator.Send(new UploadPhotoCommand {Photo = request.Photo});
                category.Photo = fileName;
            }

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.UnitOfWork.SaveChangesAsync();

            var createdCategory = _mapper.Map<CategoryDto>(category);
            return createdCategory;
        }
    }
}