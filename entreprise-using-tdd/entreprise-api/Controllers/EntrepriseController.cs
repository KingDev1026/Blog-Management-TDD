using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace entreprise_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntrepriseController : ControllerBase
    {
        private readonly List<Department> _departments;
        public EntrepriseController(List<Department> departments)
        {
            _departments = departments;
        }
        public ActionResult<List<Department>> Get()
        {
            return _departments.ToList();
        }

        [HttpGet("{Id}")]
        public ActionResult<Department> GetById(Guid? id)
        {
            if (id==null)
            {
                return new NotFoundObjectResult("id not provided");
            }
            var singleDepartment = _departments.SingleOrDefault(m => m.Id == id);
            if (singleDepartment == null )
            {
                return new NotFoundObjectResult("Department Not found");
            }

            return singleDepartment;
        }

        [HttpPost]
        public ActionResult Add(Department department)
        {
            if (!ModelState.IsValid)
            {
                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(ModelState);
            }
            department.Id = Guid.NewGuid();
            _departments.Add(department);

            return CreatedAtAction("Get", department);
        }

        [HttpDelete("{id}")]
        public ActionResult Remove(Guid id)
        {
            var departmentToDelete = _departments.SingleOrDefault(m => m.Id == id);
            if (departmentToDelete == null)
            {
                return new NotFoundObjectResult(departmentToDelete);
            }
            _departments.Remove(departmentToDelete);
            return new OkObjectResult(departmentToDelete.Title + " deleted");
        }
        

        [HttpPut("{id}")]
        public ActionResult Update(Guid? id, Department department)
        {
            if(id == null || department == null)
            {
                return new NotFoundObjectResult("One of Id or Department is missing");
            }
            var departmentToUpdate = _departments.FirstOrDefault(m => m.Id == id);
            _departments.Remove(departmentToUpdate);
            if(departmentToUpdate == null)
            {
                return new NotFoundObjectResult("The Department with the id is missing");
            }
            departmentToUpdate.Id = (Guid)id;
            departmentToUpdate.Code = department.Code;
            departmentToUpdate.Title = department.Title;
            _departments.Add(departmentToUpdate);
            return new OkObjectResult(departmentToUpdate);
        }
    }
}