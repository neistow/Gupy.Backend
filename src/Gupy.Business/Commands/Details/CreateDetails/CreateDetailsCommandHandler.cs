﻿using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Gupy.Core.Dtos;
using Gupy.Core.Exceptions;
using Gupy.Core.Interfaces.Data.Repositories;
using Gupy.Domain;
using MediatR;

namespace Gupy.Business.Commands.Details.CreateDetails
{
    public class CreateDetailsCommandHandler : IRequestHandler<CreateDetailsCommand, ShippingDetailsDto>
    {
        private readonly IShippingDetailsRepository _detailsRepository;
        private readonly ITelegramUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateDetailsCommandHandler(IShippingDetailsRepository detailsRepository,
            ITelegramUserRepository userRepository, IMapper mapper)
        {
            _detailsRepository = detailsRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ShippingDetailsDto> Handle(CreateDetailsCommand request, CancellationToken cancellationToken)
        {
            var details = _mapper.Map<ShippingDetails>(request.ShippingDetailsDto);

            var user = await _userRepository.GetAsync(details.TelegramUserId);
            if (user == null)
            {
                throw new NotFoundException(nameof(details.TelegramUserId),
                    $"User with id ({details.TelegramUserId}) doesn't exist");
            }

            await _detailsRepository.AddAsync(details);
            await _detailsRepository.UnitOfWork.SaveChangesAsync();

            var result = _mapper.Map<ShippingDetailsDto>(details);
            return result;
        }
    }
}