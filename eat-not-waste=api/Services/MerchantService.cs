using AutoMapper;
using eat_not_waste_api.Data;
using eat_not_waste_api.DTOs;
using eat_not_waste_api.Exceptions;
using eat_not_waste_api.Helpers;
using eat_not_waste_api.Models;

public class MerchantService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public MerchantService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public List<MerchantDto> GetAllMerchants()
    {
        var merchants = _context.Merchants.ToList();
        return _mapper.Map<List<MerchantDto>>(merchants);
    }

    public MerchantDto GetMerchantById(int id)
    {
        var merchant = _context.Merchants.Find(id);
        if (merchant == null)
        {
            return null;
        }
        return _mapper.Map<MerchantDto>(merchant);
    }

    public MerchantDto CreateMerchant(CreateMerchantDto createMerchantDto)
    {
        // Check if email is unique
        var existingMerchant = _context.Merchants.SingleOrDefault(m => m.Email == createMerchantDto.Email);
        if (existingMerchant != null)
        {
            throw new EmailExistsException();
        }

        // Check if email is valid
        if (!UserRegisterLoginService.IsValidEmail(createMerchantDto.Email))
        {
            throw new InvalidEmailException();
        }

        // Check if password meets complexity requirements
        if (!UserRegisterLoginService.PasswordMeetsRequirements(createMerchantDto.Password))
        {
            throw new PasswordDoesNotMeetRequirementsException();
        }
        var merchant = _mapper.Map<Merchant>(createMerchantDto);
        _context.Merchants.Add(merchant);
        _context.SaveChanges();

        // Refetch the merchant object from the database
        var merchantEntity = _context.Merchants.Find(merchant.Id);

        return _mapper.Map<MerchantDto>(merchantEntity);
    }

    public MerchantDto UpdateMerchant(int id, MerchantDto merchantDto)
    {
        var merchant = _context.Merchants.Find(id);
        if (merchant == null)
        {
            return null;
        }

        _mapper.Map(merchantDto, merchant);
        _context.SaveChanges();

        return _mapper.Map<MerchantDto>(merchant);
    }

    public bool DeleteMerchant(int id)
    {
        var merchant = _context.Merchants.Find(id);
        if (merchant == null)
        {
            return false;
        }

        _context.Merchants.Remove(merchant);
        _context.SaveChanges();

        return true;
    }
}
