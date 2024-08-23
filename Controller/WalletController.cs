using LudoGameApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly LudoDbContext _context;
    private readonly AuthenticationService _authService;

    public WalletController(LudoDbContext context, AuthenticationService authService)
    {
        _context = context;
        _authService = authService;
    }

    // GET: api/User/wallet
    [HttpGet("wallet")]
    public async Task<IActionResult> GetWallet()
    {
        var userId = await _authService.GetIdFromToken();
        if (userId == -1)
        {
            return Unauthorized("Invalid token.");
        }

        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
            return NotFound("Wallet not found.");
        }

        return Ok(new { Coins = wallet.Coins });
    }

    // PUT: api/User/wallet/add-coins
    [HttpPut("wallet/add-coins")]
    public async Task<IActionResult> AddCoins([FromBody] int coins)
    {
        if (coins <= 0)
        {
            return BadRequest("Coin amount must be positive.");
        }

        var userId = await _authService.GetIdFromToken();
        if (userId == -1)
        {
            return Unauthorized("Invalid token.");
        }

        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
            return NotFound("Wallet not found.");
        }

        wallet.Coins += coins;
        await _context.SaveChangesAsync();

        return Ok(new { Coins = wallet.Coins });
    }

    // PUT: api/User/wallet/deduct-coins
    [HttpPut("wallet/deduct-coins")]
    public async Task<IActionResult> DeductCoins([FromBody] int coins)
    {
        if (coins <= 0)
        {
            return BadRequest("Coin amount must be positive.");
        }

        var userId = await _authService.GetIdFromToken();
        if (userId == -1)
        {
            return Unauthorized("Invalid token.");
        }

        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
            return NotFound("Wallet not found.");
        }

        if (wallet.Coins < coins)
        {
            return BadRequest("Insufficient coins.");
        }

        wallet.Coins -= coins;
        await _context.SaveChangesAsync();

        return Ok(new { Coins = wallet.Coins });
    }
}
