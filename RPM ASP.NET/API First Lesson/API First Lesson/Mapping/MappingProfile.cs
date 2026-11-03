using API_First_Lesson.Models;
using AutoMapper;

namespace API_First_Lesson.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
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

        CreateMap<RoleCreateDTO, Role>()
            .ForMember(d => d.Users, o => o.Ignore());
        CreateMap<RoleUpdateDTO, Role>()
            .ForMember(d => d.Users, o => o.Ignore());

        CreateMap<UserCreateDTO, User>()
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.OrderEvents, o => o.Ignore())
            .ForMember(d => d.Orders, o => o.Ignore())
            .ForMember(d => d.Role, o => o.Ignore());
        CreateMap<UserUpdateDTO, User>()
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.OrderEvents, o => o.Ignore())
            .ForMember(d => d.Orders, o => o.Ignore())
            .ForMember(d => d.Role, o => o.Ignore());

        CreateMap<CategoryCreateDTO, Category>()
            .ForMember(d => d.Products, o => o.Ignore());
        CreateMap<CategoryUpdateDTO, Category>()
            .ForMember(d => d.Products, o => o.Ignore());

        CreateMap<ProductCreateDTO, Product>()
            .ForMember(d => d.CartItems, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.OrderEvents, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore());
        CreateMap<ProductUpdateDTO, Product>()
            .ForMember(d => d.CartItems, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.OrderEvents, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore());

        CreateMap<CartCreateDTO, Cart>()
            .ForMember(d => d.CartItems, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());
        CreateMap<CartUpdateDTO, Cart>()
            .ForMember(d => d.CartItems, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());

        CreateMap<CartItemCreateDTO, CartItem>()
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore());
        CreateMap<CartItemUpdateDTO, CartItem>()
            .ForMember(d => d.Cart, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore());

        CreateMap<OrderCreateDTO, Order>()
            .ForMember(d => d.OrderEvents, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());
        CreateMap<OrderUpdateDTO, Order>()
            .ForMember(d => d.OrderEvents, o => o.Ignore())
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());

        CreateMap<OrderItemCreateDTO, OrderItem>()
            .ForMember(d => d.Order, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore());
        CreateMap<OrderItemUpdateDTO, OrderItem>()
            .ForMember(d => d.Order, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore());

        CreateMap<OrderEventCreateDTO, OrderEvent>()
            .ForMember(d => d.Order, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());
        CreateMap<OrderEventUpdateDTO, OrderEvent>()
            .ForMember(d => d.Order, o => o.Ignore())
            .ForMember(d => d.Product, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());
    }
}
