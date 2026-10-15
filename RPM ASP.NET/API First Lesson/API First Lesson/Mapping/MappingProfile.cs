using API_First_Lesson.Models;
using AutoMapper;

namespace API_First_Lesson.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        AllowNullCollections = true;

        CreateMap<Role, RoleDTO>();
        CreateMap<Category, CategoryDTO>();

        CreateMap<User, UserDTO>()
            .ForMember(d => d.RoleName, o => o.MapFrom(s => s.Role == null ? null : s.Role.RoleName))
            .ForMember(d => d.CartId, o => o.MapFrom(s => s.Cart == null ? (int?)null : s.Cart.CartId));

        CreateMap<Product, ProductDTO>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category == null ? null : s.Category.CategoryName));

        CreateMap<Cart, CartDTO>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User == null ? null : s.User.Username));

        CreateMap<CartItem, CartItemDTO>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product == null ? null : s.Product.ProductName));

        CreateMap<Order, OrderDTO>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User == null ? null : s.User.Username));

        CreateMap<OrderItem, OrderItemDTO>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product == null ? null : s.Product.ProductName));

        CreateMap<OrderEvent, OrderEventDTO>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User == null ? null : s.User.Username))
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product == null ? null : s.Product.ProductName));

        CreateMap<RoleCreateDTO, Role>();
        CreateMap<RoleUpdateDTO, Role>();

        CreateMap<UserCreateDTO, User>();
        CreateMap<UserUpdateDTO, User>();

        CreateMap<CategoryCreateDTO, Category>();
        CreateMap<CategoryUpdateDTO, Category>();

        CreateMap<ProductCreateDTO, Product>();
        CreateMap<ProductUpdateDTO, Product>();

        CreateMap<CartCreateDTO, Cart>();
        CreateMap<CartUpdateDTO, Cart>();

        CreateMap<CartItemCreateDTO, CartItem>();
        CreateMap<CartItemUpdateDTO, CartItem>();

        CreateMap<OrderCreateDTO, Order>();
        CreateMap<OrderUpdateDTO, Order>();

        CreateMap<OrderItemCreateDTO, OrderItem>();
        CreateMap<OrderItemUpdateDTO, OrderItem>();

        CreateMap<OrderEventCreateDTO, OrderEvent>();
        CreateMap<OrderEventUpdateDTO, OrderEvent>();
    }
}
