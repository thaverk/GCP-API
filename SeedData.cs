using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();

                string[] roleNames = { "SuperAdmin", "Admin", "Staff", "Athlete" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Create a default admin user
                
                List<double>? RMList =new List<double> { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
                List<double>? RPEList = new List<double> { 0.0, 0.5, 1.0, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0, 5.5, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 10.0};

                List<string> RIRList = new List<string>
                    { 
                    "Light Effort",
                    "Light Effort",
                    "Light Effort",
                    "Light Effort",
                    "Light Effort",
                    "Light Effort",
                    "Med Effort",
                    "Med Effort",
                    "Med Effort",
                    "Med Effort",
                    "4-6",
                    "4-6",
                    "4-6",
                    "4-6",
                    "3-4",
                    "3-4",
                    "1-2",
                    "1-2",
                    "0-3",
                    "0-1",
                    "0-1"
                };

                List<string>VelocityList = new List<string>
                { 
                ">1.3",
                ">1.3",
                ">1.3",
                ">1.3",
                ">1.3",
                "1.0 - 1.3",
                "1.0 - 1.3",
                "1.0 - 1.3",
                "1.0 - 1.3",
                "1.0 - 1.3",
                "0.75 - 1.0",
                "0.75 - 1.0",
                "0.75 - 1.0",
                "0.75 - 1.0",
                "0.75 - 1.0",
                "0.5 - 0.75",
                "0.5 - 0.75",
                "0.5 - 0.75",
                "<0.5",
                "<0.5",
                "<0.5",
                };

                // Add PercentRM_Mapping data
                for (int i = 0; i < RMList.Count; i++)
                {
                    var mapping = new PercentRM_Mapping
                    {
                        PercentRM = RMList[i], // Example value - adjust as needed
                        RPE = RPEList[i],        // Example value - adjust as needed
                        RIR = RIRList[i],      // Example value - adjust as needed
                        VelocityRange = VelocityList[i] // Example value - adjust as needed
                    };

                    var existing = await dbContext.PercentRM_Mapping
                    .FirstOrDefaultAsync(m => m.PercentRM == mapping.PercentRM);

                    // Check if mapping exists
                    if (existing == null)
                    {
                        dbContext.PercentRM_Mapping.Add(mapping);
                        await dbContext.SaveChangesAsync();
                    }
                }

                
            }
        }
    }
}
