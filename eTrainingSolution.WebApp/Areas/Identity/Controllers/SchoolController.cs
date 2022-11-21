﻿using eTrainingSolution.EntityFrameworkCore;
using eTrainingSolution.EntityFrameworkCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTrainingSolution.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("/School/[action]")]
    public class SchoolController : Controller
    {

        private readonly eTrainingDbContext _eTrainingDbContext;

        public SchoolController(eTrainingDbContext eTrainingDbContext)
        {
            _eTrainingDbContext = eTrainingDbContext;
        }

        /// <summary>
        /// Sự kiện khi click vào School trên menu
        /// </summary>
        /// <returns>Index.cshtml</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // sử dụng toán tử 2 ngôi. Nếu mà không null thì trả về danh sách
            return _eTrainingDbContext.Schools != null ? View(await _eTrainingDbContext.Schools.ToListAsync()) : Problem("null");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, SchoolName, Address, CreateDate, CapacityOfTheSchool")] School school)
        {
            if (ModelState.IsValid)
            {
                school.Id = Guid.NewGuid();
                _eTrainingDbContext.Add(school);
                await _eTrainingDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _eTrainingDbContext.Schools == null)
            {
                return NotFound();
            }    
            var school = await _eTrainingDbContext.Schools.FindAsync(id);
            if (school == null)
            {
                return NotFound();
            }
            return View(school);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, SchoolName, Address, CreateDate, CapacityOfTheSchool")] School school)
        {
            if (id != school.Id)
            {
                // trả về kết quả không tìm thấy
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // bản chất của _eTrainingDbContext.Update là kiểm tra xem Enity có null không và sau đó gán là Modified
                    _eTrainingDbContext.Update(school);
                    await _eTrainingDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _eTrainingDbContext.Schools == null)
            {
                return NotFound();
            }
            var school = await _eTrainingDbContext.Schools.FirstOrDefaultAsync(m => m.Id == id);
            if (school == null)
            {
                return NotFound();
            }
            return View(school);
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid? id)
        {
            if (id == null || _eTrainingDbContext.Schools == null)
            {
                return NotFound();
            }
            var school = await _eTrainingDbContext.Schools.FirstOrDefaultAsync(m => m.Id == id);
            if (school == null)
            {
                return NotFound();
            }
            return View(school);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            if(_eTrainingDbContext.Schools == null)
            {
                return Problem("Entity set null");
            }
            // tìm kiếm thông tin trường học theo id
            var schoolDbContext = await _eTrainingDbContext.Schools.FindAsync(id);
            if( schoolDbContext != null)
            {
                // lấy ra danh sách các Khoa theo id trường học
                var facultuesDb = await _eTrainingDbContext.Facultys.Where(m => m.SchoolID == id).ToListAsync();
                if(facultuesDb != null)
                {
                    // Duyệt Khoa để lấy ra thông tin của lớp học
                    foreach(var faculties in facultuesDb)
                    {
                        // lấy ra danh sách lớp của Khoa
                        var classesDbContext =await _eTrainingDbContext.Classrooms.Where(m => m.FacultyID == faculties.ID).ToListAsync();
                        if (classesDbContext != null)
                        {
                            _eTrainingDbContext.RemoveRange(classesDbContext);
                        }
                        _eTrainingDbContext.Remove(faculties);
                    }
                }
                _eTrainingDbContext.Remove(schoolDbContext);
            }
            await _eTrainingDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
