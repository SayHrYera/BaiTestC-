using App_Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace App_MVC.Controllers
{
    public class NhanVienController : Controller
    {
        exam_distribution_testContext db;
        public NhanVienController()
        {
            db = new exam_distribution_testContext();
        }
        [HttpGet]
        public IActionResult Index()
        {
            var ListNv = db.staff.ToList();
            return View(ListNv);
        }
        [HttpGet]
        public IActionResult CreateStaff()
        {
            return View();
        }
        [HttpPost]
        [HttpPost]
        public IActionResult CreateStaff(staff model, DateTimeOffset date)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicates
                if (db.staff.Any(s => s.AccountFe == model.AccountFe || s.AccountFpt == model.AccountFpt || s.StaffCode == model.StaffCode))
                {
                    ModelState.AddModelError("", "Mã hoặc email đã tồn tại.");
                }

                // Validate email formats
                if (!model.AccountFe.EndsWith("@fe.edu.vn"))
                {
                    ModelState.AddModelError("AccountFe", "Email FE phải đúng định dạng đuôi @fe.edu.vn.");
                }

                if (!model.AccountFpt.EndsWith("@fpt.edu.vn"))
                {
                    ModelState.AddModelError("AccountFpt", "Email FPT phải đúng định dạng đuôi @fpt.edu.vn.");
                }

                // Validate no whitespace and Vietnamese characters
                if (model.AccountFe.Any(c => char.IsWhiteSpace(c)) || model.AccountFpt.Any(c => char.IsWhiteSpace(c)))
                {
                    ModelState.AddModelError("", "Email không được chứa khoảng trắng.");
                }

                if (ContainsVietnameseCharacters(model.AccountFe) || ContainsVietnameseCharacters(model.AccountFpt))
                {
                    ModelState.AddModelError("", "Email không được chứa tiếng Việt.");
                }

                // Validate length
                if (model.StaffCode.Length > 15)
                {
                    ModelState.AddModelError("", "Độ dài mã vượt quá giới hạn 15 kí tự.");
                }

                if (model.AccountFe.Length > 100 || model.AccountFpt.Length > 100)
                {
                    ModelState.AddModelError("", "Độ dài email vượt quá giới hạn 100 kí tự.");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Add new staff
                model.Id = Guid.NewGuid();
                model.Status = 1;
                model.CreatedDate = ToUnixTimestamp(DateTime.Now);

                db.staff.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditStaff(Guid id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var staff = db.staff.Find(id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }
        [HttpPost]
        public IActionResult EditStaff(staff model, Guid id)
        {
            if (ModelState.IsValid)
            {
                var existingStaff = db.staff.Find(id);
                if (existingStaff == null)
                {
                    return NotFound();
                }

                // Check for duplicates
                if (db.staff.Any(s => (s.AccountFe == model.AccountFe || s.AccountFpt == model.AccountFpt || s.StaffCode == model.StaffCode) && s.Id != id))
                {
                    ModelState.AddModelError("", "Mã nhân viên hoặc email đã tồn tại.");
                }

                // Validate email formats and lengths
                if (!model.AccountFe.EndsWith("@fe.edu.vn"))
                {
                    ModelState.AddModelError("AccountFe", "Email FE phải đúng định dạng đuôi @fe.edu.vn.");
                }

                if (!model.AccountFpt.EndsWith("@fpt.edu.vn"))
                {
                    ModelState.AddModelError("AccountFpt", "Email FPT phải đúng định dạng đuôi @fpt.edu.vn.");
                }

                if (model.AccountFe.Any(c => char.IsWhiteSpace(c)) || model.AccountFpt.Any(c => char.IsWhiteSpace(c)))
                {
                    ModelState.AddModelError("", "Email không được chứa khoảng trắng.");
                }

                if (ContainsVietnameseCharacters(model.AccountFe) || ContainsVietnameseCharacters(model.AccountFpt))
                {
                    ModelState.AddModelError("", "Email không được chứa tiếng Việt.");
                }

                if (model.StaffCode.Length > 15 || model.AccountFe.Length > 100 || model.AccountFpt.Length > 100)
                {
                    ModelState.AddModelError("", "Độ dài mã nhân viên hoặc email vượt quá giới hạn cho phép.");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Update existing staff
                existingStaff.StaffCode = model.StaffCode;
                existingStaff.Name = model.Name;
                existingStaff.AccountFe = model.AccountFe;
                existingStaff.AccountFpt = model.AccountFpt;
                existingStaff.Status = model.Status;
                existingStaff.LastModifiedDate = ToUnixTimestamp(DateTime.Now);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangeStatus(Guid id)
        {
            var staff = db.staff.Find(id);
            if (staff == null)
            {
                return NotFound();
            }

            staff.Status = (byte?)(staff.Status != 1 ? 1 : 2);
            staff.LastModifiedDate = ToUnixTimestamp(DateTime.Now);

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult DetailStaff(Guid id)
        {
            var staff = db.staff
                .Include(s => s.StaffMajorFacilities)
                    .ThenInclude(smf => smf.IdMajorFacilityNavigation)
                        .ThenInclude(mf => mf.IdMajorNavigation)
                .Include(s => s.StaffMajorFacilities)
                    .ThenInclude(smf => smf.IdMajorFacilityNavigation)
                        .ThenInclude(mf => mf.IdDepartmentFacilityNavigation)
                            .ThenInclude(df => df.IdFacilityNavigation)
                .FirstOrDefault(s => s.Id == id);

            if (staff == null)
            {
                return NotFound();
            }

            // Get the list of facility IDs that the staff member is already associated with
            var associatedFacilityIds = staff.StaffMajorFacilities
                .Select(smf => smf.IdMajorFacilityNavigation.IdDepartmentFacilityNavigation.IdFacilityNavigation.Id)
                .Distinct()
                .ToList();

            // Filter out the associated facilities
            ViewData["Facilities"] = db.Facilities
                .Where(f => !associatedFacilityIds.Contains(f.Id))
                .ToList();

            // Filter MajorFacilities to exclude those already associated with the staff
            ViewData["MajorFacilities"] = db.MajorFacilities
                .Include(mf => mf.IdDepartmentFacilityNavigation)
                    .ThenInclude(df => df.IdFacilityNavigation)
                .Where(mf => !associatedFacilityIds.Contains(mf.IdDepartmentFacilityNavigation.IdFacilityNavigation.Id))
                .ToList();

            ViewData["Majors"] = db.Majors.ToList();

            return View(staff);
        }

        [HttpPost]
        public IActionResult AddMajorFacility(Guid staffId, Guid facilityId, Guid majorFacilityId, Guid majorId)
        {
            var staff = db.staff.Find(staffId);
            if (staff == null)
            {
                return NotFound();
            }

            var majorFacility = db.MajorFacilities
                .Include(mf => mf.IdDepartmentFacilityNavigation)
                .FirstOrDefault(mf => mf.Id == majorFacilityId);

            if (majorFacility == null)
            {
                return NotFound();
            }

            var existingStaffMajorFacility = db.StaffMajorFacilities
                .Any(smf => smf.IdStaff == staffId &&
                                 smf.IdMajorFacilityNavigation.IdDepartmentFacilityNavigation.IdFacility == majorFacility.IdDepartmentFacilityNavigation.IdFacility);

            if (existingStaffMajorFacility)
            {
                ModelState.AddModelError("", "Nhân viên đã có bộ môn chuyên ngành trong cơ sở này.");
                ViewData["Facilities"] = db.Facilities.ToList();
                return View("DetailStaff", staff);
            }

            // Thêm bộ môn chuyên ngành mới
            var staffMajorFacility = new StaffMajorFacility
            {
                Id = Guid.NewGuid(),
                IdStaff = staffId,
                IdMajorFacility = majorFacilityId
            };

            db.StaffMajorFacilities.Add(staffMajorFacility);
            db.SaveChanges();

            return RedirectToAction("DetailStaff", new { id = staffId });
        }
        [HttpGet]
        public IActionResult RemoveMajorFacility(Guid staffId, Guid majorFacilityId)
        {
            var staffMajorFacility = db.StaffMajorFacilities
                .FirstOrDefault(smf => smf.IdStaff == staffId && smf.IdMajorFacility == majorFacilityId);

            if (staffMajorFacility == null)
            {
                return NotFound();
            }

            db.StaffMajorFacilities.Remove(staffMajorFacility);
            db.SaveChanges();

            return RedirectToAction("DetailStaff", new { id = staffId });
        }
        [HttpGet]
        public IActionResult GetMajorFacilities(Guid facilityId)
        {
            var majorFacilities = db.MajorFacilities
                .Where(mf => mf.IdDepartmentFacilityNavigation.IdFacility == facilityId)
                .Select(mf => new { id = mf.Id, name = mf.IdMajorNavigation.Name })
                .ToList();

            return Json(majorFacilities);
        }
        [HttpGet]
        public IActionResult GetMajors(Guid majorFacilityId)
        {
            var majors = db.Majors
                .Where(m => db.MajorFacilities.Any(mf => mf.Id == majorFacilityId && mf.IdMajor == m.Id))
                .Select(m => new { id = m.Id, name = m.Name })
                .ToList();

            return Json(majors);
        }
        [HttpGet]
        public IActionResult DownloadTemplate()
        {
            var template = db.staff.ToList();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh sách nhân viên");
                worksheet.Cells[1, 1].Value = "Mã Nhân Viên";
                worksheet.Cells[1, 2].Value = "Tên Nhân Viên";
                worksheet.Cells[1, 3].Value = "Tình Trạng";
                worksheet.Cells[1, 4].Value = "Email FPT";
                worksheet.Cells[1, 5].Value = "Email FE";

                for (int i = 0; i < template.Count; i++)
                {
                    var staff = template[i];
                    worksheet.Cells[i + 2, 1].Value = staff.StaffCode;
                    worksheet.Cells[i + 2, 2].Value = staff.Name;
                    worksheet.Cells[i + 2, 3].Value = staff.Status == 1 ? "Đang hoạt động" : "Ngừng hoạt động";
                    worksheet.Cells[i + 2, 4].Value = staff.AccountFpt;
                    worksheet.Cells[i + 2, 5].Value = staff.AccountFe;
                }
                worksheet.Cells.AutoFitColumns();
                var folderPath = @"D:\DanhSachNhanVien";
                var fileName = $"DanhSachNhanVien_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                System.IO.File.WriteAllBytes(filePath, package.GetAsByteArray());
            }
            return Content("Danh sách nhân viên đã được lưu thành công.");
        }

        [HttpPost]
        public IActionResult ImportTemplate(IFormFile fileUpload)
        {
            if (fileUpload == null || fileUpload.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file Excel.";
                return RedirectToAction("Index");
            }

            var employees = new List<staff>();
            var errorMessages = new List<string>();
            string fileName = Path.GetFileName(fileUpload.FileName);
            int totalRecords = 0;
            int successCount = 0;
            int failureCount = 0;

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(fileUpload.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var staffCode = worksheet.Cells[row, 1].Text.Trim();
                        var name = worksheet.Cells[row, 2].Text.Trim();
                        var statusText = worksheet.Cells[row, 3].Text.Trim();
                        var accountFpt = worksheet.Cells[row, 4].Text.Trim();
                        var accountFe = worksheet.Cells[row, 5].Text.Trim();

                        // Validate data
                        if (string.IsNullOrWhiteSpace(staffCode) ||
                            string.IsNullOrWhiteSpace(name) ||
                            string.IsNullOrWhiteSpace(accountFpt) ||
                            string.IsNullOrWhiteSpace(accountFe))
                        {
                            errorMessages.Add($"Dòng {row}: Dữ liệu không hợp lệ.");
                            failureCount++;
                            continue;
                        }

                        byte statusValue;
                        switch (statusText.ToLower())
                        {
                            case "đang hoạt động":
                                statusValue = 1;
                                break;
                            case "ngừng hoạt động":
                                statusValue = 2;
                                break;
                            default:
                                errorMessages.Add($"Dòng {row}: Trạng thái không hợp lệ. Phải là 'Đang hoạt động' hoặc 'Ngừng hoạt động'.");
                                failureCount++;
                                continue;
                        }

                        if (!accountFe.EndsWith("@fe.edu.vn"))
                        {
                            errorMessages.Add($"Dòng {row}: Email FE phải đúng định dạng đuôi @fe.edu.vn.");
                            failureCount++;
                            continue;
                        }

                        if (!accountFpt.EndsWith("@fpt.edu.vn"))
                        {
                            errorMessages.Add($"Dòng {row}: Email FPT phải đúng định dạng đuôi @fpt.edu.vn.");
                            failureCount++;
                            continue;
                        }

                        if (db.staff.Any(s => s.StaffCode == staffCode || s.AccountFe == accountFe || s.AccountFpt == accountFpt))
                        {
                            errorMessages.Add($"Dòng {row}: Mã nhân viên hoặc email đã tồn tại.");
                            failureCount++;
                            continue;
                        }

                        var stafff = new staff
                        {
                            Id = Guid.NewGuid(),
                            StaffCode = staffCode,
                            Name = name,
                            AccountFpt = accountFpt,
                            AccountFe = accountFe,
                            Status = statusValue,
                            CreatedDate = ToUnixTimestamp(DateTime.Now)
                        };

                        employees.Add(stafff);
                        successCount++;
                        totalRecords++;
                    }

                    if (employees.Any())
                    {
                        db.staff.AddRange(employees);
                        db.SaveChanges();
                    }

                    var importHistory = new ImportHistory
                    {
                        ImportDate = DateTime.UtcNow,
                        FileName = fileName,
                        TotalRecords = totalRecords,
                        SuccessfulRecords = successCount,
                        FailedRecords = failureCount,
                        ErrorDetails = string.Join(" | ", errorMessages)
                    };

                    db.ImportHistories.Add(importHistory);
                    db.SaveChanges();

                    if (errorMessages.Any())
                    {
                        TempData["ErrorMessage"] = $"Một số lỗi xảy ra: {string.Join(" | ", errorMessages)}";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Dữ liệu đã được import thành công!";
                    }
                }
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi xử lý file Excel: {ex.Message}. Inner exception: {innerExceptionMessage}";
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult ImportHistory()
        {
            var importHistories = db.ImportHistories
            .OrderByDescending(h => h.ImportDate)
            .ToList();
            return View(importHistories);
        }

        public long ToUnixTimestamp(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime - epoch).TotalSeconds;
        }
        private bool ContainsVietnameseCharacters(string input)
        {
            var vietnameseChars = "àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ";
            return input.Any(c => vietnameseChars.Contains(c));
        }
    }
}
