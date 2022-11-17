using Bvs.Entities;
using Bvs_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bvs_API.DTOs;
using Bvs_API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Bvs_API.Controllers
{   
    [Authorize]
    public class StudentController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentController(DataContext context, IStudentRepository studentRepository, IMapper mapper)
        {
            _context = context;
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return Ok(await _studentRepository.GetStudentsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            return await _studentRepository.GetStudentByIdAsync(id);
        }

        [HttpPost("addStudent")]
        public async Task<ActionResult<Student>> AddStudent(StudentDto studentDto)
        {
            if (await _studentRepository.CheckMatrikelnummer(studentDto.MatrikelNum))
            {
                return BadRequest($"Matrikelnummer {studentDto.MatrikelNum} ist bereits existiert!");
            }

            if (await _studentRepository.CheckBibNummer(studentDto.BibNum))
            {
                return BadRequest($"Bibliotheknummber {studentDto.BibNum} ist bereits existiert!");
            }

            Student student = _mapper.Map<Student>(studentDto);
            student.Foto = "";

            _studentRepository.AddStudent(student);
            bool isSave = await _studentRepository.SaveAllAsync();

            if (isSave)
            {
                await StudentCount();
                await _studentRepository.SaveAllAsync();
            }

            return student;
        }

        [HttpPut("editStudent")]
        public async Task<ActionResult> EditStudent(Student st)
        {
            var student = await _studentRepository.GetStudentByIdAsync(st.Id);

            if (student == null)
            {
                return NotFound($"Student mit Id= {st.Id} wurde nicht gefunden.");
            }

            if (await _studentRepository.CheckMatrikelnummer(st.MatrikelNum) && student.MatrikelNum != st.MatrikelNum)
            {
                return BadRequest($"Matrikelnummer {st.MatrikelNum} ist bereits existiert!");
            }

            if (await _studentRepository.CheckBibNummer(st.BibNum) && student.BibNum != st.BibNum)
            {
                return BadRequest($"Bibliotheknummber {st.BibNum} ist bereits existiert!");
            }

            _mapper.Map(st, student);

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);

            if (student == null)
            {
                return NotFound($"Student mit Id= {id} wurde nicht gefunden.");
            }
            
            bool hasBorrow = await _context.Borrow.AnyAsync(x => x.students == student);

            if (hasBorrow)
            {
                return BadRequest("Dieses Student hat noch Bücher in Ausleihe!");
            }

            _studentRepository.RemoveStudent(student);
            bool isSave = await _studentRepository.SaveAllAsync();

            if (isSave)
            {
                await StudentCount();
                await _studentRepository.SaveAllAsync();

                return Ok();
            }

            return BadRequest("Somthing went wrong");
        }

        // count methode for student
        private async Task StudentCount()
        {
            var studentCount = await _context.Student.CountAsync();
            var numberOverview = await _context.NumberOverview.FirstAsync();
            numberOverview.AnzahlStudent = studentCount;
        }

    }
}
