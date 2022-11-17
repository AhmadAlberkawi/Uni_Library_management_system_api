using AutoMapper;
using Bvs.Entities;
using Bvs_API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {   
            // Admin Mapping

            CreateMap<Admin, AdminDto>();
            CreateMap<Admin, AdminTokenDto>();
            CreateMap<AdminRegisterDto, Admin>();
            CreateMap<AdminEditDto, Admin>();

            // Student Mapping
            CreateMap<Student, Student>();
            CreateMap<StudentDto, Student>();

            // Book Mapping
            CreateMap<BookDto, Book>();
            CreateMap<Book, Book>();

            // Borrow Mapping
            //CreateMap<BorrowDto, Borrow>()
            //    .ForMember(dest => dest.students, 
            //    opt => opt.MapFrom(src => src.StudentId))
            //    .ForMember(dest => dest.Books, 
            //    opt => opt.MapFrom(src => src.BookId));

            //CreateMap<Borrow, BorrowListDto>()
            //    .ForMember(dest => dest.Id,
            //    opt => opt.MapFrom(src => src.Id))
            //    .ForMember(dest => dest.Foto,
            //    opt => opt.MapFrom(src => src.Books.B_foto))
            //    .ForMember(dest => dest.Title,
            //    opt => opt.MapFrom(src => src.Books.Title))
            //    .ForMember(dest => dest.Isbn,
            //    opt => opt.MapFrom(src => src.Books.Isbn))
            //    .ForMember(dest => dest.Verlag,
            //    opt => opt.MapFrom(src => src.Books.Verlag))
            //    .ForMember(dest => dest.Autro,
            //    opt => opt.MapFrom(src => src.Books.Autor))
            //    .ForMember(dest => dest.StudentName,
            //    opt => opt.MapFrom(src => src.students.Name))
            //    .ForMember(dest => dest.BorrowedUntil,
            //    opt => opt.MapFrom(src => src.BorrowedUntil.ToString("d")))
            //    .ForMember(dest => dest.RemainingDays,
            //    opt => opt.MapFrom(src => src.GetremainingDays()));
        }
    }
}
