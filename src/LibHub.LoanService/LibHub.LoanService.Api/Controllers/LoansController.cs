using LibHub.LoanService.Application.DTOs;
using LibHub.LoanService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibHub.LoanService.Api.Controllers;

[ApiController]
[Route("api/loans")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly LoanApplicationService _loanService;

    public LoansController(LoanApplicationService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost]
    public async Task<IActionResult> BorrowBook(CreateLoanDto createLoanDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (userId == 0)
                return Unauthorized("User ID not found in token");

            var loanDto = await _loanService.BorrowBookAsync(userId, createLoanDto);
            return CreatedAtAction(nameof(GetLoanById), new { id = loanDto.LoanId }, loanDto);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not available"))
                return Conflict(ex.Message);
            
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/return")]
    public async Task<IActionResult> ReturnBook(int id)
    {
        try
        {
            await _loanService.ReturnBookAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
                return NotFound(ex.Message);
            
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserLoans(int userId)
    {
        try
        {
            var loans = await _loanService.GetUserLoansAsync(userId);
            return Ok(loans);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLoanById(int id)
    {
        return Ok(new { message = "Loan details endpoint - implementation pending" });
    }
}
