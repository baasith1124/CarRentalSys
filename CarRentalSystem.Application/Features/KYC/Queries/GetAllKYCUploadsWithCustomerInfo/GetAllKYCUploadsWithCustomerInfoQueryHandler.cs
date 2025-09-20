using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.KYC;
using CarRentalSystem.Domain.Entities;
using MediatR;

namespace CarRentalSystem.Application.Features.KYC.Queries.GetAllKYCUploadsWithCustomerInfo
{
    public class GetAllKYCUploadsWithCustomerInfoQueryHandler : IRequestHandler<GetAllKYCUploadsWithCustomerInfoQuery, List<KYCUploadDto>>
    {
        private readonly IKYCRepository _kycRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public GetAllKYCUploadsWithCustomerInfoQueryHandler(
            IKYCRepository kycRepository, 
            ICustomerRepository customerRepository,
            IMapper mapper)
        {
            _kycRepository = kycRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<List<KYCUploadDto>> Handle(GetAllKYCUploadsWithCustomerInfoQuery request, CancellationToken cancellationToken)
        {
            var kycUploads = await _kycRepository.GetAllKYCUploadsAsync(cancellationToken);
            var kycDtos = new List<KYCUploadDto>();

            foreach (var kyc in kycUploads)
            {
                var kycDto = _mapper.Map<KYCUploadDto>(kyc);
                
                // Get customer information
                var customer = await _customerRepository.GetCustomerByIdAsync(kyc.UserId, cancellationToken);
                if (customer != null)
                {
                    kycDto.UserName = customer.FullName;
                    kycDto.UserEmail = customer.Email;
                }
                else
                {
                    kycDto.UserName = "Unknown Customer";
                    kycDto.UserEmail = "Unknown Email";
                }
                
                kycDtos.Add(kycDto);
            }

            return kycDtos;
        }
    }
}
