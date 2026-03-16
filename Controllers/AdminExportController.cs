using AccommodationManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AccommodationManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminExportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ExportAllData()
        {
            ExcelPackage.License.SetNonCommercialPersonal("AccommodationManagement");

            using var package = new ExcelPackage();

            //------------------------------------
            // 1️⃣ Users Sheet
            //------------------------------------
            var users = _context.Users.ToList();

            var userSheet = package.Workbook.Worksheets.Add("Users");

            userSheet.Cells[1, 1].Value = "Full Name";
            userSheet.Cells[1, 2].Value = "Email";
            userSheet.Cells[1, 3].Value = "Phone";
            userSheet.Cells[1, 4].Value = "Emergency Contact";
            userSheet.Cells[1, 5].Value = "Join Date";

            int row = 2;

            foreach (var u in users)
            {
                userSheet.Cells[row, 1].Value = u.FullName;
                userSheet.Cells[row, 2].Value = u.Email;
                userSheet.Cells[row, 3].Value = u.Phone;
                userSheet.Cells[row, 4].Value = u.EmergencyContact;
                userSheet.Cells[row, 5].Value = u.CreatedDate;

                row++;
            }

            //------------------------------------
            // 2️⃣ Rooms Sheet
            //------------------------------------
            var rooms = _context.Rooms.ToList();

            var roomSheet = package.Workbook.Worksheets.Add("Rooms");

            roomSheet.Cells[1, 1].Value = "Room Number";
            roomSheet.Cells[1, 2].Value = "Total Beds";
            roomSheet.Cells[1, 3].Value = "Occupied Beds";
            roomSheet.Cells[1, 4].Value = "Vacant Beds";

            row = 2;

            foreach (var r in rooms)
            {
                roomSheet.Cells[row, 1].Value = r.RoomNumber;
                roomSheet.Cells[row, 2].Value = r.TotalBeds;
                roomSheet.Cells[row, 3].Value = r.OccupiedBeds;
                roomSheet.Cells[row, 4].Value = r.VacantBeds;
                row++;
            }

            //------------------------------------
            // 3️⃣ Beds Sheet
            //------------------------------------
            var beds = _context.Beds.ToList();

            var bedSheet = package.Workbook.Worksheets.Add("Beds");

            bedSheet.Cells[1, 1].Value = "Bed Number";
            bedSheet.Cells[1, 2].Value = "Room Id";
            bedSheet.Cells[1, 3].Value = "Occupied";

            row = 2;

            foreach (var b in beds)
            {
                bedSheet.Cells[row, 1].Value = b.BedNumber;
                bedSheet.Cells[row, 2].Value = b.RoomId;
                bedSheet.Cells[row, 3].Value = b.IsOccupied;
                row++;
            }

            //------------------------------------
            // 4️⃣ Gate Logs Sheet
            //------------------------------------
            var logs = _context.GateEntryLogs.ToList();

            var logSheet = package.Workbook.Worksheets.Add("Gate Logs");

            logSheet.Cells[1, 1].Value = "UserId";
            logSheet.Cells[1, 2].Value = "Action";
            logSheet.Cells[1, 3].Value = "Scan Time";

            row = 2;

            foreach (var l in logs)
            {
                logSheet.Cells[row, 1].Value = l.UserId;
                logSheet.Cells[row, 2].Value = l.Action;
                logSheet.Cells[row, 3].Value = l.ScanTime;

                row++;
            }

            //------------------------------------
            // Generate Excel File
            //------------------------------------
            var fileBytes = package.GetAsByteArray();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "HostelFullData.xlsx"
            );
        }
    }
}
