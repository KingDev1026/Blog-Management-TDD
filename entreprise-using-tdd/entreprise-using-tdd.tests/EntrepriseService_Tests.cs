using Xunit;
using entreprise_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using entreprise_api;
using System.Collections.Generic;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace entreprise_using_tdd.tests
{
    public class EntrepriseService_Tests
    {
        private readonly EntrepriseController _entrepriseController;
        private Mock<List<Department>> _mockDepartmentsList;
       

        public EntrepriseService_Tests()
        {
            _mockDepartmentsList = new Mock<List<Department>>();
            _entrepriseController = new EntrepriseController(_mockDepartmentsList.Object);
        }

        [Fact]
        public void GetTest_ReturnsListofDepartments()
        {
            //arrange
            var mockDepartments = new List<Department> {
                new Department{Title = "Facturation"},
                new Department{Title = "Facturation and new Facturation"}
            };

            _mockDepartmentsList.Object.AddRange(mockDepartments);

            //act
            var result = _entrepriseController.Get();

            //assert
           var model = Assert.IsAssignableFrom<ActionResult<List<Department>>>(result);
            Assert.Equal(2, model.Value.Count);
        }

        [Fact]
        public void GetByIdTest_ReturnsNotFound_WhenIdNotProvided()
        {
            //act 
            var result = _entrepriseController.GetById(null);

            //asert
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetByIdTest_ReturnsNotFound_WhenDepartmentDoesNotExit()
        {
            //arrange
            var department = new Department() { Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad") };

            _mockDepartmentsList.Object.SingleOrDefault(m => m.Id == department.Id);

            //act
            var result = _entrepriseController.GetById(department.Id);

            //assert
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetByIdTest_ReturnsSingleDepartment_WhenDepartmentExist()
        {
            //arrange 
            var singleMockDepartment = new Department() { Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f") ,
                Title ="facturation", Code ="Facturation"};

            _mockDepartmentsList.Object.Add(singleMockDepartment);

            //act 
            var result = _entrepriseController.GetById(singleMockDepartment.Id);

            //assert 
            var model = Assert.IsType<ActionResult<Department>>(result);
            Assert.Equal(singleMockDepartment, model.Value);
        }

        [Fact]
        public void AddTest_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            //arrange 
            var titleMissing = new Department() { Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"), Code = "Facturation" };

            _entrepriseController.ModelState.AddModelError("Title", "Title field is required");

            //act 
            var result = _entrepriseController.Add(titleMissing);

            //assert 
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public void AddTest_ReturnsCreatedResponse_WhenValidObjectPassed()
        {
            //arrange
            var mockDepartment = new Department() { Code="Facturation",
                Title = "NEW MESSAGES"
            };

            //act 
            mockDepartment.Id = Guid.NewGuid();
            var result = _entrepriseController.Add(mockDepartment);

            //assert
            Assert.IsAssignableFrom<CreatedAtActionResult>(result);
        }

        [Fact]
        public void AddTest_ReturnsResponseHasCreatedItem_WhenValidObjectPassed()
        {
            //arrange 
            var mockDepartment = new Department() { Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"), Code="Facturation",
                    Title = "Begining to learn is the fun part about being learned"};

            //act
            var result = _entrepriseController.Add(mockDepartment) as CreatedAtActionResult;
            var item = result.Value as Department;

            //assert 
            Assert.IsType<Department>(item);
            Assert.Equal("Facturation", item.Code);
        }

        [Fact]
        public void RemoveTest_ReturnsNotFound_WhenGuidNotExisting()
        {
            //arrange
            var notExistingGuid = Guid.NewGuid();

            //act
            var result = _entrepriseController.Remove(notExistingGuid);

            //assert 
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void RemoveTest_ReturnsOkResult_WhenGuidIsExisting()
        {
            //arrange 
            var mockDepartment = new Department()
            {
                Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
                Code = "Facturation",
                Title = "Begining to learn is the fun part about being learned"
            };
            _mockDepartmentsList.Object.Add(mockDepartment);


            //act
            var result = _entrepriseController.Remove(mockDepartment.Id);

            //assert
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public void RemoveTest_RemovesOneItem_WhenGuidIsExisting()
        {
            var mockDepartment = new List<Department>()
            {
                new Department(){Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"), Title = "Facturation One" },
                new Department(){Id = Guid.NewGuid(), Title = "Facturation Two" },
                new Department(){Id = Guid.NewGuid(), Title = "Facturation Three"}
            };
            _mockDepartmentsList.Object.AddRange(mockDepartment);

            //act 
            _entrepriseController.Remove(new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"));

            //assert
            Assert.Equal(2,_entrepriseController.Get().Value.Count());
        }

        [Fact]
        public void UpdateTest_ReturnsNull_WhenIdAndDepartmentAreNull()
        {
            //act
            var result = _entrepriseController.Update(null,null);

            //assert
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnsNull_WhenIdIsNotNullAndDepartmentIsNull()
        {
            //arrange 
            var mockDepartmentId = Guid.NewGuid();

            //act 
            var result = _entrepriseController.Update(mockDepartmentId, null);

            //assert 
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnsNull_WhenIdIsNullAndDepartmentIsNotNull()
        {
            //arrange 
            var mockDepartment = new Department() { Id = Guid.NewGuid(), Title = "Facturation two" };

            //act
            var result = _entrepriseController.Update(null, mockDepartment);

            //assert 
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnNotFoundResult_WhenIdNotExisting()
        {
            //arrange 
            var mockDepartment = new Department() { Id = Guid.NewGuid(), Title = "Facturation Three" };

            //act
            var result = _entrepriseController.Update(mockDepartment.Id, mockDepartment);

            //assert
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnsOkResult_WhenIdIsPresent()
        {
            //arrange 
            var mockDepartment = new Department() { Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
                Title = "Facturation", Code = "Affaire"
            };

            _mockDepartmentsList.Object.Add(mockDepartment);

            //act
            var result = _entrepriseController.Update(mockDepartment.Id, mockDepartment);

            //assert
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnsNewItemAfterUpdate_WhenIdIsPresent()
        {
            //arrange 
            var mockDepartment = new Department()
            {
                Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
                Title = "Facturation",
                Code = "Learn well"
            };
            _mockDepartmentsList.Object.Add(mockDepartment);

            var mockDepartmentToUpdate = new Department()
            {
                Title = "Facturation",
                Code = "Affaire"
            };

            //act
            var result = _entrepriseController.Update(mockDepartment.Id, mockDepartmentToUpdate);

            //assert
            var model = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal(mockDepartmentToUpdate.Code, _entrepriseController.GetById(mockDepartment.Id).Value.Code);
        }
    }
}
