using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;   
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            var walks = await walkRepository.GetAllWalksAsync();

            //var walksDTO = new List<Models.DTO.Walk>();

            //walks.ToList().ForEach(walk =>
            //{
            //    var walkDTO = new Models.DTO.Walk()
            //    {
            //        Id = walk.Id,
            //        Name = walk.Name,
            //        Length = walk.Length,
            //        RegionId = walk.RegionId,
            //        WalkDifficultyId = walk.WalkDifficultyId,
            //    };
            //    walksDTO.Add(walkDTO);
            //});
            //return Ok(walksDTO);

            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walks);

            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalksByIdAsync")]
        public async Task<IActionResult> GetWalksByIdAsync(Guid id)
        {
            var walks = await walkRepository.GetWalkAsync(id);

            if (walks == null)
            {
                return NotFound();
            }

            var walksDTO = mapper.Map<Models.DTO.Walk>(walks);
            return Ok(walksDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync(Models.DTO.AddWalkRequest addWalkRequest)
        {
            var walk = new Models.Domain.Walk()
            {
                Name = addWalkRequest.Name,
                Length = addWalkRequest.Length,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId,
            };

            walk = await walkRepository.AddAsync(walk);

            var walkDTO = new Models.DTO.Walk()
            {
                Id = walk.Id,
                Name = walk.Name,
                Length = walk.Length,
                RegionId = walk.RegionId,
                WalkDifficultyId = walk.WalkDifficultyId,
            };

            return CreatedAtAction(nameof(GetWalksByIdAsync), new { id = walkDTO.Id }, walkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, [FromBody] Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            //Convert DTO to domain model
            var walk = new Models.Domain.Walk()
            {
                Name = updateWalkRequest.Name,
                Length = updateWalkRequest.Length,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId,
            };

            //Update Walk using repository
            walk = await walkRepository.UpdateAsync(id, walk);

            //If null then NotFound
            if(walk == null)
            {
                return NotFound();
            }

            //Convert Domain back to DTO
            var walkDTO = new Models.DTO.Walk()
            {
                Id = walk.Id,
                Name = walk.Name,
                Length = walk.Length,
                RegionId = walk.RegionId,
                WalkDifficultyId = walk.WalkDifficultyId
            };

            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id)
        {
            //Get walk from database
            var walk = await walkRepository.DeleteAsync(id);

            //If null not found
            if(walk == null)
            {
                return NotFound();
            }

            //Convert response back to DTO
            var walkDTO = new Models.DTO.Walk()
            {
                Id = walk.Id,
                Name = walk.Name,
                Length = walk.Length,
                RegionId = walk.RegionId,
                WalkDifficultyId = walk.WalkDifficultyId
            };

            //Return the response
            return Ok(walkDTO);
        }
    }
}
