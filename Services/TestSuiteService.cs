﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OneTestApi.Models;

namespace OneTestApi.Services
{
    public class AddTestSuiteParams
    {
        public int ParentSuiteId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }


    public class UpdateTestSuiteParams
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public interface ITestSuiteService
    {
        TestSuite GetTestSuite(int suiteId);

        TestSuite GetTestSuiteDetail(int suiteId);

        TestSuite GetRootTestSuite(int projectId);

        int AddTestSuite(AddTestSuiteParams ps);

        void UpdateTestSuite(UpdateTestSuiteParams ps);
    }

    public class TestSuiteService : ITestSuiteService
    {
        private OneTestDbContext _context;

        public TestSuiteService(OneTestDbContext context)
        {
            _context = context;
        }

        public TestSuite GetTestSuite(int suiteId)
        {
            return _context.TestSuites.Single(ts => ts.Id == suiteId);
        }

        public TestSuite GetTestSuiteDetail(int suiteId)
        {
            return _context.TestSuites.Include(ts => ts.TestSuites).Include(ts => ts.TestCases)
                .Single(ts => ts.Id == suiteId);
        }

        public TestSuite GetRootTestSuite(int projectId)
        {
            return _context.TestProjects.Include(tp => tp.TestSuites)
                .Single(tp => tp.Id == projectId).TestSuites.First();
        }

        public int AddTestSuite(AddTestSuiteParams ps)
        {
            var parent = _context.TestSuites.Include(ts => ts.TestProject).Single(ts => ts.Id == ps.ParentSuiteId);

            var testSuite = new TestSuite()
            {
                Name = ps.Name,
                Description = ps.Description,
                Order = 0,
                Count = 0,
                ParentTestSuite = parent,
                TestProject = parent.TestProject
            };

            parent.TestSuites.Add(testSuite);

            _context.SaveChanges();

            return testSuite.Id;
        }

        public void UpdateTestSuite(UpdateTestSuiteParams ps)
        {
            var testSuite = _context.TestSuites.Single(ts => ts.Id == ps.Id);

            testSuite.Name = ps.Name;
            testSuite.Description = ps.Description;

            _context.SaveChanges();
        }
    }
}