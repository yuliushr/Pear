﻿using DSLNG.PEAR.Data.Persistence;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Data.Entities;

namespace DSLNG.PEAR.Data.Installer
{
    public class RoleGroupsInstaller
    {
        private DataContext _context;
        public RoleGroupsInstaller(DataContext context)
        {
            _context = context;
        }

        public void Install() {
            var groupPlanningDirectorate = new RoleGroup
            {
                Id = 1,
                Name = "Planning Directorate",
                Level = _context.Levels.Local.Where(x => x.Id == 1).First(),
                IsActive = true
            };
            var groupOperationDirectorate = new RoleGroup
            {
                Id = 1,
                Name = "Operation Directorate",
                Level = _context.Levels.Local.Where(x => x.Id == 1).First(),
                IsActive = true
            };
            var groupFinanceDirectorate = new RoleGroup
            {
                Id = 1,
                Name = "Finance Directorate",
                Level = _context.Levels.Local.Where(x => x.Id == 1).First(),
                IsActive = true
            };
            var groupTechnicalDirectorate = new RoleGroup
            {
                Id = 1,
                Name = "Technical Directorate",
                Level = _context.Levels.Local.Where(x => x.Id == 1).First(),
                IsActive = true
            };
            var groupCommercialDirectorate = new RoleGroup
            {
                Id = 1,
                Name = "Commercial Directorate",
                Level = _context.Levels.Local.Where(x => x.Id == 1).First(),
                IsActive = true
            };
            var groupCAffairDirectorate = new RoleGroup
            {
                Id = 1,
                Name = "Corporate Affair Directorate",
                Level = _context.Levels.Local.Where(x => x.Id == 1).First(),
                IsActive = true
            };
            var groupPresdir = new RoleGroup
            {
                Id = 1,
                Name = "President Director Office",
                Level = _context.Levels.Local.Where(x => x.Id == 1).First(),
                IsActive = true
            };
            var groupHCMDirectorate = new RoleGroup
            {
                Id = 1,
                Name = "Human Capital Management",
                Level = _context.Levels.Local.Where(x => x.Id == 2).FirstOrDefault(),
                IsActive = true
            };
            var groupProcurement = new RoleGroup
            {
                Id = 1,
                Name = "Procurement",
                Level = _context.Levels.Local.Where(x => x.Id == 1).FirstOrDefault(),
                IsActive = true
            };
            var groupQHSE = new RoleGroup
            {
                Id = 1,
                Name = "QHSE",
                Level = _context.Levels.Local.Where(x => x.Id == 2).FirstOrDefault(),
                IsActive = true
            };
            var groupIT = new RoleGroup
            {
                Id = 1,
                Name = "Information Communication & Tech",
                Level = _context.Levels.Local.Where(x => x.Id == 2).FirstOrDefault(),
                IsActive = true
            };
            var groupCSR = new RoleGroup
            {
                Id = 1,
                Name = "Community Support & Relation",
                Level = _context.Levels.Local.Where(x => x.Id == 2).FirstOrDefault(),
                IsActive = true
            };
            _context.RoleGroups.Add(groupPlanningDirectorate);
            _context.RoleGroups.Add(groupOperationDirectorate);
            _context.RoleGroups.Add(groupFinanceDirectorate);
            _context.RoleGroups.Add(groupTechnicalDirectorate);
            _context.RoleGroups.Add(groupCommercialDirectorate);
            _context.RoleGroups.Add(groupCAffairDirectorate);
            _context.RoleGroups.Add(groupPresdir);
            _context.RoleGroups.Add(groupHCMDirectorate);
            _context.RoleGroups.Add(groupProcurement);
            _context.RoleGroups.Add(groupQHSE);
            _context.RoleGroups.Add(groupIT);
            _context.RoleGroups.Add(groupCSR);
        }
    }
}
