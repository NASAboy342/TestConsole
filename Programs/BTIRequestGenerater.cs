using System.Net.Http.Headers;
using System.Text;

namespace TestConsole.Programs;

public class BTIRequestGenerater
{
    // private const string BaseUrl = "https://capi-uat-btisports.techbodia.dev";
    private const string BaseUrl = "https://btisports-prod.csmc-api.com";
    private const string AuthToken = "YOUR_TOKEN_HERE";
    private const string DomainId = "2544";

    private static readonly HttpClient _http = new();
    private static readonly Random _rng = new();

    private static long Gen18() =>
        long.Parse(
            $"{_rng.Next(1, 10)}" +
            $"{_rng.Next(0, 10000):D4}" +
            $"{_rng.Next(0, 10000):D4}" +
            $"{_rng.Next(0, 10000):D4}" +
            $"{_rng.Next(0, 10000):D4}" +
            $"{_rng.Next(0, 10):D1}");

    private static long Gen9() =>
        long.Parse(
            $"{_rng.Next(1, 10)}" +
            $"{_rng.Next(0, 10000):D4}" +
            $"{_rng.Next(0, 10000):D4}");

    // ── Input helpers ─────────────────────────────────────────────────────────
    private static (string username, string betAmount, string creditAmount) PromptInput()
    {
        Console.Clear();
        Console.WriteLine("┌─────────────────────────────────────┐");
        Console.WriteLine("│       BTI Request Generator          │");
        Console.WriteLine("└─────────────────────────────────────┘");
        Console.Write("  Username      : ");
        string username = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(username)) throw new ArgumentException("Username is required.");

        Console.Write("  Bet amount    : ");
        string betAmount = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(betAmount)) throw new ArgumentException("Bet amount is required.");

        Console.Write("  Credit amount : ");
        string creditAmount = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(creditAmount)) throw new ArgumentException("Credit amount is required.");

        return (username, betAmount, creditAmount);
    }

    private record Ids(
        string Username, string BetAmount, string CreditAmount,
        long ReserveId, long AgentId, long CustomerId,
        long PurchaseId, long BetId, long RequestId, long PurchaseBetId,
        long CreditReqId1, long CreditReqId2, long CreditReqId3);

    private static Ids GenerateIds(string username, string betAmount, string creditAmount)
    {
        long reserveId     = Gen18();
        long agentId       = Gen9();
        long customerId    = Gen9();
        long purchaseId    = Gen18();
        long betId         = purchaseId + 1;
        long requestId     = Gen18();
        long purchaseBetId = purchaseId;
        long creditReqId1  = Gen18();
        long creditReqId2  = Gen18();
        long creditReqId3  = Gen18();
        return new Ids(username, betAmount, creditAmount,
            reserveId, agentId, customerId,
            purchaseId, betId, requestId, purchaseBetId,
            creditReqId1, creditReqId2, creditReqId3);
    }

    // ── Menu ──────────────────────────────────────────────────────────────────
    public async Task Run()
    {
        var (username, betAmount, creditAmount) = PromptInput();
        Ids ids = GenerateIds(username, betAmount, creditAmount);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("┌──────────────────────────────────────────────────┐");
            Console.WriteLine($"│  User: {ids.Username,-20} Bet: {ids.BetAmount,-8} Credit: {ids.CreditAmount,-6} ReserveId: {ids.ReserveId,-10} │");
            Console.WriteLine("├──────────────────────────────────────────────────┤");
            Console.WriteLine("│  [1]  Reserve                                    │");
            Console.WriteLine("│  [2]  DebitReserve                               │");
            Console.WriteLine("│  [3]  CommitReserve                              │");
            Console.WriteLine($"│  [4]  CreditCustomer — settlement 1 ({GetNewStatus(ids.BetAmount, ids.CreditAmount)})       │");
            Console.WriteLine("│  [5]  CreditCustomer — settlement 2 (Resettled)  │");
            Console.WriteLine($"│  [6]  CreditCustomer — settlement 3 (Final/{GetNewStatus(ids.BetAmount, ids.CreditAmount)}) │");
            Console.WriteLine($"│  [7]   TryCreditCustomer - settlement ({GetNewStatus(ids.BetAmount, ids.CreditAmount)}) │");
            Console.WriteLine("│  [8]  Reserve (custom amount)                    │");
            Console.WriteLine("│  [9]  CreditCustomer (custom credit amount)       │");
            Console.WriteLine("│  [A]  All (1–6 in order)                         │");
            Console.WriteLine("│  [M]  MixParlay — All legs (V-Soccer, 3 legs)    │");
            Console.WriteLine("│  [P]  MixParlay — Rollback a leg (→ Opened)      │");
            Console.WriteLine("│  [S]  MixParlay — Resettle a leg (Won/Lost)      │");
            Console.WriteLine("│  [C]  Cashout (single bet)                       │");
            Console.WriteLine("│  [R]  Re-generate IDs                            │");
            Console.WriteLine("│  [I]  Change inputs                              │");
            Console.WriteLine("│  [Q]  Quit                                       │");
            Console.WriteLine("└──────────────────────────────────────────────────┘");
            Console.Write("  Choice: ");

            string choice = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";
            Console.WriteLine();

            switch (choice)
            {
                case "1": await CallReserve(ids);         break;
                case "2": await CallDebitReserve(ids);    break;
                case "3": await CallCommitReserve(ids);   break;
                case "4": await CallCredit1(ids);         break;
                case "5": await CallCredit2(ids);         break;
                case "6": await CallCredit3(ids);         break;
                case "7": await CallCredit7(ids);         break;
                case "8": await CallReserveCustomAmount(ids); break;
                case "9": await CallCreditCustomAmount(ids);  break;
                case "A":
                    await CallReserve(ids);
                    await CallDebitReserve(ids);
                    await CallCommitReserve(ids);
                    await CallCredit1(ids);
                    await CallCredit2(ids);
                    await CallCredit3(ids);
                    break;
                case "M":
                    await CallMpReserve(ids);
                    await CallMpDebitReserve(ids);
                    await CallMpCommitReserve(ids);
                    await CallMpCreditLeg1(ids);
                    await CallMpCreditLeg2(ids);
                    await CallMpCreditLeg3(ids);
                    break;
                case "P": await CallMpRollbackLeg(ids);  break;
                case "S": await CallMpResettleLeg(ids);  break;
                case "C": await CallCashout(ids);         break;
                case "R":
                    ids = GenerateIds(ids.Username, ids.BetAmount, ids.CreditAmount);
                    Console.WriteLine("  New IDs generated.");
                    break;
                case "I":
                    (username, betAmount, creditAmount) = PromptInput();
                    ids = GenerateIds(username, betAmount, creditAmount);
                    break;
                case "Q":
                    return;
                default:
                    Console.WriteLine("  Invalid choice. Press any key to continue.");
                    break;
            }

            if (choice != "I")
            {
                Console.WriteLine();
                Console.Write("  Press any key to return to menu...");
                Console.ReadKey(intercept: true);
            }
        }
    }

    // ── CreditCustomer with a custom credit amount ────────────────────────────
    private static async Task CallCreditCustomAmount(Ids i)
    {
        var (username, betAmount, _, reserveId, agentId, customerId, purchaseId, betId, _, _, _, _, _) = i;
        Console.Write("  Custom credit amount : ");
        string customCredit = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(customCredit)) { Console.WriteLine("  Cancelled."); return; }
        long reqId = Gen18();
        string status = GetNewStatus(betAmount, customCredit).ToString()!;
        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={reqId}&amount={customCredit}&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="{customCredit}" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="{customCredit}" CreationDateUTC="2026-04-22T20:50:23.850" CurrentBalance="0" seq_num="4" CurrentStatus="Closed" NumberOfLines="3" NumberOfOpenLines="2" NumberOfSettledLines="1" NumberOfLostLines="1" NumberOfWonLines="0" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="1705283464" EventName="Bournemouth vs Leeds" EventTypeName="Live Betting O/U" LeagueName="England - Premier League" YourBet="Under 3.5" HomeTeam="Bournemouth" AwayTeam="Leeds" DecimalOdds="1.41" UserOddStyle="Decimal" OddsInUserStyle="1.41" BranchID="1" BranchName="Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="5517581541" MasterLeagueID="24" Score="2:1" EventTypeID="39" EventDateUTC="2026-04-22 19:00" MasterEventID="1428514984" EventID="1428514984" NewMasterEventID="830846952735604736" NewEventID="830846952735604736" NewLeagueID="740557258543472640" NewLineID="0OU830846955327684687UMM" EncodedID="0OU830846955327684687UMM" EventState="" BestOddsApplied="0" SelectionID="1" DBOdds="1.41" IsLive="1" CurrentResult="3 : 1" MarketID="0OU830846955327684687">
          <Changes>
            <Change ID="834893893542944815" OldStatus="Opened" NewStatus="{status}" Amount="{customCredit}" PrevBalance="0" NewBalance="0" DateUTC="2026-04-22T20:50:23" TriggeredResult="3 : 1">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="{customCredit}" OldStatus="Opened" NewStatus="{status}" IsFreeBet="0" MaxPayout="3780853" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="2026-04-22T20:50:23" IsResettlement="0"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        Console.WriteLine($"  req_id : {reqId}");
        await SendAsync($"CreditCustomer (custom credit: {customCredit} - {status})", url, xml);
    }

    // ── Input for custom amount (does not modify ids) ───────────────────────
    private static async Task CallReserveCustomAmount(Ids i)
    {
        var (username, _, _, reserveId, agentId, customerId, _, _, _, _, _, _, _) = i;
        Console.Write("  Wrong amount to send : ");
        string wrongAmount = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(wrongAmount)) { Console.WriteLine("  Cancelled."); return; }
        string url = $"{BaseUrl}/Reserve?reserve_id={reserveId}&Amount={wrongAmount}&IsFreeBet=false&agent_id={agentId}&customer_id={customerId}&cust_id={username}";
        string xml = $"""
<Bets cust_id="{customerId}" reserve_id="{reserveId}" amount="{wrongAmount}.00" bonus_amount="0.0000" real_amount="{wrongAmount}.0000" ip_address="1.47.30.13" country="TH">
  <Bet BetID="0" BetTypeID="2" BetTypeName="Trebles X 1 bet" Stake="{wrongAmount}.0000" OrgStake="{wrongAmount}.0000" Gain="1751.1200" StakeTax="0" IsLive="1" NumberOfBets="1" Status="Opened" IsFreeBet="0" BonusID="0" FreebetAmount="0" RealAmount="{wrongAmount}.0000" BonusAmount="0.0000" MaxPayout="3780853" CreationDate="2026-04-22T20:45:25" PurchaseBetID="0" Odds="217" OddsDec="3.1600" ComboBetNumersLines="3" ReferenceID="0" ReserveAmountType="Real" ReserveAmountTypeID="1">
    <Lines LineID="1705283464" Stake="{wrongAmount}" OddsDec="1.4100" Gain="1751.12" LiveScore1="2" LiveScore2="1" HomeTeam="Bournemouth" AwayTeam="Leeds" Status="Opened" EventState="" CustomerID="{customerId}" BetID="0" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="1" BranchName="Soccer" LeagueID="93522362" LeagueName="England - Premier League" MasterLeagueID="24" CreationDate="2026-04-22T20:45:25" YourBet="Under 3.5" EventTypeID="39" EventTypeName="Live Betting O/U" EventDate="2026-04-22T19:00:00" MasterEventID="1428514984" EventID="1428514984" NewMasterEventID="830846952735604736" NewEventID="830846952735604736" NewLeagueID="740557258543472640" NewLineID="0OU830846955327684687UMM" EventName="Bournemouth vs Leeds" BetTypeID="2" Odds="-244" DBOdds="1.41" Score="2:1" IsLive="1" Points="3.5" />
    <Lines LineID="1424232857" Stake="{wrongAmount}" OddsDec="1.9700" Gain="1751.12" LiveScore1="1" LiveScore2="0" HomeTeam="Barcelona" AwayTeam="Celta Vigo" Status="Opened" EventState="" CustomerID="{customerId}" BetID="0" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="1" BranchName="Soccer" LeagueID="755876133" LeagueName="Spain - La Liga" MasterLeagueID="38" CreationDate="2026-04-22T20:45:25" YourBet="Under 2.75" EventTypeID="39" EventTypeName="Live Betting O/U" EventDate="2026-04-22T19:30:00" MasterEventID="640647454" EventID="640647454" NewMasterEventID="828718296756142080" NewEventID="828718296756142080" NewLeagueID="740557503432101888" NewLineID="0OU828718301323739214UMM" EventName="Barcelona vs Celta Vigo" BetTypeID="2" Odds="-103" DBOdds="1.97" Score="1:0" IsLive="1" Points="2.75" />
    <Lines LineID="292294103" Stake="{wrongAmount}" OddsDec="1.1400" Gain="1751.12" LiveScore1="1" LiveScore2="2" HomeTeam="Charlton" AwayTeam="Ipswich" Status="Opened" EventState="" CustomerID="{customerId}" BetID="0" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="1" BranchName="Soccer" LeagueID="1346648161" LeagueName="England - Championship" MasterLeagueID="43" CreationDate="2026-04-22T20:45:25" YourBet="Under 3.5" EventTypeID="39" EventTypeName="Live Betting O/U" EventDate="2026-04-22T18:45:00" MasterEventID="1440155981" EventID="1440155981" NewMasterEventID="831241131676508160" NewEventID="831241131676508160" NewLeagueID="726053182400376832" NewLineID="0OU831241133106708545UMM" EventName="Charlton vs Ipswich" BetTypeID="2" Odds="-714" DBOdds="1.14" Score="1:2" IsLive="1" Points="3.5" />
  </Bet>
</Bets>
""";
        await SendAsync($"Reserve (custom amount: {wrongAmount})", url, xml, withAuth: true);
    }

    // ── HTTP helper ───────────────────────────────────────────────────────────
    private static async Task SendAsync(string label, string url, string? xmlBody = null, bool withAuth = false, HttpMethod? method = null)
    {
        Console.WriteLine($"  Calling {label}...");
        try
        {
            var request = new HttpRequestMessage(method ?? HttpMethod.Post, url);
            if (withAuth)
                request.Headers.Authorization = new AuthenticationHeaderValue(AuthToken);

            if (xmlBody is not null)
                request.Content = xmlBody.Length > 0
                    ? new StringContent(xmlBody, Encoding.UTF8, "application/xml")
                    : new StringContent(""); // empty body like curl --data ''

            var response = await _http.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"  Status : {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"  Response:");
            Console.WriteLine(body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
        }
    }

    // ── Call methods ──────────────────────────────────────────────────────────
    private static async Task CallReserve(Ids i)
    {
        var (username, betAmount, _, reserveId, agentId, customerId, _, _, _, _, _, _, _) = i;
        string url = $"{BaseUrl}/Reserve?reserve_id={reserveId}&Amount={betAmount}&IsFreeBet=false&agent_id={agentId}&customer_id={customerId}&cust_id={username}";
        string xml = $"""
<Bets cust_id="{customerId}" reserve_id="{reserveId}" amount="{betAmount}.00" bonus_amount="0.0000" real_amount="{betAmount}.0000" ip_address="1.47.30.13" country="TH">
  <Bet BetID="0" BetTypeID="2" BetTypeName="Trebles X 1 bet" Stake="{betAmount}.0000" OrgStake="{betAmount}.0000" Gain="1751.1200" StakeTax="0" IsLive="1" NumberOfBets="1" Status="Opened" IsFreeBet="0" BonusID="0" FreebetAmount="0" RealAmount="{betAmount}.0000" BonusAmount="0.0000" MaxPayout="3780853" CreationDate="2026-04-22T20:45:25" PurchaseBetID="0" Odds="217" OddsDec="3.1600" ComboBetNumersLines="3" ReferenceID="0" ReserveAmountType="Real" ReserveAmountTypeID="1">
    <Lines LineID="1705283464" Stake="{betAmount}" OddsDec="1.4100" Gain="1751.12" LiveScore1="2" LiveScore2="1" HomeTeam="Bournemouth" AwayTeam="Leeds" Status="Opened" EventState="" CustomerID="{customerId}" BetID="0" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="1" BranchName="Soccer" LeagueID="93522362" LeagueName="England - Premier League" MasterLeagueID="24" CreationDate="2026-04-22T20:45:25" YourBet="Under 3.5" EventTypeID="39" EventTypeName="Live Betting O/U" EventDate="2026-04-22T19:00:00" MasterEventID="1428514984" EventID="1428514984" NewMasterEventID="830846952735604736" NewEventID="830846952735604736" NewLeagueID="740557258543472640" NewLineID="0OU830846955327684687UMM" EventName="Bournemouth vs Leeds" BetTypeID="2" Odds="-244" DBOdds="1.41" Score="2:1" IsLive="1" Points="3.5" />
    <Lines LineID="1424232857" Stake="{betAmount}" OddsDec="1.9700" Gain="1751.12" LiveScore1="1" LiveScore2="0" HomeTeam="Barcelona" AwayTeam="Celta Vigo" Status="Opened" EventState="" CustomerID="{customerId}" BetID="0" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="1" BranchName="Soccer" LeagueID="755876133" LeagueName="Spain - La Liga" MasterLeagueID="38" CreationDate="2026-04-22T20:45:25" YourBet="Under 2.75" EventTypeID="39" EventTypeName="Live Betting O/U" EventDate="2026-04-22T19:30:00" MasterEventID="640647454" EventID="640647454" NewMasterEventID="828718296756142080" NewEventID="828718296756142080" NewLeagueID="740557503432101888" NewLineID="0OU828718301323739214UMM" EventName="Barcelona vs Celta Vigo" BetTypeID="2" Odds="-103" DBOdds="1.97" Score="1:0" IsLive="1" Points="2.75" />
    <Lines LineID="292294103" Stake="{betAmount}" OddsDec="1.1400" Gain="1751.12" LiveScore1="1" LiveScore2="2" HomeTeam="Charlton" AwayTeam="Ipswich" Status="Opened" EventState="" CustomerID="{customerId}" BetID="0" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="1" BranchName="Soccer" LeagueID="1346648161" LeagueName="England - Championship" MasterLeagueID="43" CreationDate="2026-04-22T20:45:25" YourBet="Under 3.5" EventTypeID="39" EventTypeName="Live Betting O/U" EventDate="2026-04-22T18:45:00" MasterEventID="1440155981" EventID="1440155981" NewMasterEventID="831241131676508160" NewEventID="831241131676508160" NewLeagueID="726053182400376832" NewLineID="0OU831241133106708545UMM" EventName="Charlton vs Ipswich" BetTypeID="2" Odds="-714" DBOdds="1.14" Score="1:2" IsLive="1" Points="3.5" />
  </Bet>
</Bets>
""";
        await SendAsync("Reserve", url, xml, withAuth: true);
    }

    private static async Task CallDebitReserve(Ids i)
    {
        var (username, betAmount, _, reserveId, agentId, customerId, _, betId, requestId, purchaseBetId, _, _, _) = i;
        requestId = Gen18(); // DebitReserve requires a different requestId than Reserve, so we generate a new one here
        string url = $"{BaseUrl}/DebitReserve?req_id={requestId}&reserve_id={reserveId}&amount={betAmount}&IsFreeBet=false&AgentId={agentId}&CustomerId={customerId}&Username={username}&reserve_id={reserveId}&agent_id={agentId}&customer_id={customerId}&cust_id={username}";
        string xml = $"""
<Bets cust_id="{customerId}" reserve_id="{reserveId}" amount="{betAmount}.00" bonus_amount="0.0000" real_amount="{betAmount}.0000" ip_address="1.47.30.13" country="TH" language_code="th">
  <Bet BetID="{betId}" BetTypeID="2" BetTypeName="Trebles X 1 bet" Stake="{betAmount}.0000" OrgStake="{betAmount}.0000" Gain="1751.1200" StakeTax="0" IsLive="1" NumberOfBets="1" Status="Opened" IsFreeBet="0" BonusID="0" FreebetAmount="0" RealAmount="{betAmount}.0000" BonusAmount="0.0000" MaxPayout="3780853" CreationDate="2026-04-22T20:45:25" PurchaseBetID="{purchaseBetId}" Odds="217" OddsDec="3.1600" ComboBetNumersLines="3" UserOddStyle="Decimal" ReserveAmountType="Real" ReserveAmountTypeID="1">
    <Lines LineID="1705283464" Stake="{betAmount}" OddsDec="1.4100" Gain="1751.12" LiveScore1="2" LiveScore2="1" HomeTeam="Bournemouth" AwayTeam="Leeds" Status="Opened" EventState="" CustomerID="{customerId}" BetID="{betId}" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="1" BranchName="Soccer" LeagueID="93522362" LeagueName="England - Premier League" MasterLeagueID="24" CreationDate="2026-04-22T20:45:25" YourBet="Under 3.5" EventTypeID="39" EventTypeName="Live Betting O/U" EventDate="2026-04-22T19:00:00" MasterEventID="1428514984" EventID="1428514984" NewMasterEventID="830846952735604736" NewEventID="830846952735604736" NewLeagueID="740557258543472640" NewLineID="0OU830846955327684687UMM" EventName="Bournemouth vs Leeds" BetTypeID="2" Odds="-244" DBOdds="1.41" Score="2:1" IsLive="1" Points="3.5" />
    <Lines LineID="1424232857" Stake="{betAmount}" OddsDec="1.9700" Gain="1751.12" LiveScore1="1" LiveScore2="0" HomeTeam="Barcelona" AwayTeam="Celta Vigo" Status="Opened" EventState="" CustomerID="{customerId}" BetID="{betId}" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="1" BranchName="Soccer" LeagueID="755876133" LeagueName="Spain - La Liga" MasterLeagueID="38" CreationDate="2026-04-22T20:45:25" YourBet="Under 2.75" EventTypeID="39" EventTypeName="Live Betting O/U" EventDate="2026-04-22T19:30:00" MasterEventID="640647454" EventID="640647454" NewMasterEventID="828718296756142080" NewEventID="828718296756142080" NewLeagueID="740557503432101888" NewLineID="0OU828718301323739214UMM" EventName="Barcelona vs Celta Vigo" BetTypeID="2" Odds="-103" DBOdds="1.97" Score="1:0" IsLive="1" Points="2.75" />
    <Lines LineID="292294103" Stake="{betAmount}" OddsDec="1.1400" Gain="1751.12" LiveScore1="1" LiveScore2="2" HomeTeam="Charlton" AwayTeam="Ipswich" Status="Opened" EventState="" CustomerID="{customerId}" BetID="{betId}" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="1" BranchName="Soccer" LeagueID="1346648161" LeagueName="England - Championship" MasterLeagueID="43" CreationDate="2026-04-22T20:45:25" YourBet="Under 3.5" EventTypeID="39" EventTypeName="Live Betting O/U" EventDate="2026-04-22T18:45:00" MasterEventID="1440155981" EventID="1440155981" NewMasterEventID="831241131676508160" NewEventID="831241131676508160" NewLeagueID="726053182400376832" NewLineID="0OU831241133106708545UMM" EventName="Charlton vs Ipswich" BetTypeID="2" Odds="-714" DBOdds="1.14" Score="1:2" IsLive="1" Points="3.5" />
  </Bet>
</Bets>
""";
        await SendAsync("DebitReserve", url, xml, withAuth: true);
    }

    private static async Task CallCommitReserve(Ids i)
    {
        var (username, _, _, reserveId, agentId, customerId, _, _, _, purchaseBetId, _, _, _) = i;
        Console.WriteLine($"  reserve_id : {reserveId}");
        string url = $"{BaseUrl}/CommitReserve?cust_id={username}&reserve_id={reserveId}&agent_id={agentId}&customer_id={customerId}&purchase_id={purchaseBetId}";
        await SendAsync("CommitReserve", url, method: HttpMethod.Get);
    }

    private static async Task CallCredit1(Ids i)
    {
        var (username, betAmount, creditAmount, reserveId, agentId, customerId, purchaseId, betId, _, _, creditReqId1, _, _) = i;
        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={creditReqId1}&amount={creditAmount}&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="{creditAmount}" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="{creditAmount}" CreationDateUTC="2026-04-22T20:50:23.850" CurrentBalance="0" seq_num="4" CurrentStatus="Closed" NumberOfLines="3" NumberOfOpenLines="2" NumberOfSettledLines="1" NumberOfLostLines="1" NumberOfWonLines="0" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="1705283464" EventName="Bournemouth vs Leeds" EventTypeName="Live Betting O/U" LeagueName="England - Premier League" YourBet="Under 3.5" HomeTeam="Bournemouth" AwayTeam="Leeds" DecimalOdds="1.41" UserOddStyle="Decimal" OddsInUserStyle="1.41" BranchID="1" BranchName="Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="5517581541" MasterLeagueID="24" Score="2:1" EventTypeID="39" EventDateUTC="2026-04-22 19:00" MasterEventID="1428514984" EventID="1428514984" NewMasterEventID="830846952735604736" NewEventID="830846952735604736" NewLeagueID="740557258543472640" NewLineID="0OU830846955327684687UMM" EncodedID="0OU830846955327684687UMM" EventState="" BestOddsApplied="0" SelectionID="1" DBOdds="1.41" IsLive="1" CurrentResult="3 : 1" MarketID="0OU830846955327684687">
          <Changes>
            <Change ID="834893893542944815" OldStatus="Opened" NewStatus="{GetNewStatus(betAmount, creditAmount)}" Amount="{creditAmount}" PrevBalance="0" NewBalance="0" DateUTC="2026-04-22T20:50:23" TriggeredResult="3 : 1">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="{creditAmount}" OldStatus="Opened" NewStatus="{GetNewStatus(betAmount, creditAmount)}" IsFreeBet="0" MaxPayout="3780853" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="2026-04-22T20:50:23" IsResettlement="0"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        await SendAsync($"CreditCustomer (1 - {GetNewStatus(betAmount, creditAmount)})", url, xml);
    }

    private static async Task CallCredit7(Ids i)
    {
        var (username, betAmount, creditAmount, reserveId, agentId, customerId, purchaseId, betId, _, _, _, _, _) = i;
        long reqId = Gen18();
        string status = GetNewStatus(betAmount, creditAmount).ToString()!;
        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={reqId}&amount={creditAmount}&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="{creditAmount}" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="{creditAmount}" CreationDateUTC="2026-04-22T20:50:23.850" CurrentBalance="0" seq_num="4" CurrentStatus="Closed" NumberOfLines="3" NumberOfOpenLines="2" NumberOfSettledLines="1" NumberOfLostLines="1" NumberOfWonLines="0" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="1705283464" EventName="Bournemouth vs Leeds" EventTypeName="Live Betting O/U" LeagueName="England - Premier League" YourBet="Under 3.5" HomeTeam="Bournemouth" AwayTeam="Leeds" DecimalOdds="1.41" UserOddStyle="Decimal" OddsInUserStyle="1.41" BranchID="1" BranchName="Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="5517581541" MasterLeagueID="24" Score="2:1" EventTypeID="39" EventDateUTC="2026-04-22 19:00" MasterEventID="1428514984" EventID="1428514984" NewMasterEventID="830846952735604736" NewEventID="830846952735604736" NewLeagueID="740557258543472640" NewLineID="0OU830846955327684687UMM" EncodedID="0OU830846955327684687UMM" EventState="" BestOddsApplied="0" SelectionID="1" DBOdds="1.41" IsLive="1" CurrentResult="3 : 1" MarketID="0OU830846955327684687">
          <Changes>
            <Change ID="834893893542944815" OldStatus="{status}" NewStatus="{status}" Amount="{creditAmount}" PrevBalance="0" NewBalance="0" DateUTC="2026-04-22T20:50:23" TriggeredResult="3 : 1">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="{creditAmount}" OldStatus="{status}" NewStatus="{status}" IsFreeBet="0" MaxPayout="3780853" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="2026-04-22T20:50:23" IsResettlement="0"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        Console.WriteLine($"  req_id : {reqId}");
        await SendAsync($"CreditCustomer (Try - {status} -> {status})", url, xml);
    }

    private static object GetNewStatus(string betAmount, string creditAmount)
    {
        if (decimal.TryParse(betAmount, out decimal bet) && decimal.TryParse(creditAmount, out decimal credit))
        {
            return credit >= bet ? "Won" : "Lost";
        }
        return "Unknown";
    }

    private static async Task CallCredit2(Ids i)
    {
        var (username, betAmount, creditAmount, reserveId, agentId, customerId, purchaseId, betId, _, _, _, creditReqId2, _) = i;
        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={creditReqId2}&amount={creditAmount}&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="{creditAmount}" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="{creditAmount}" CreationDateUTC="2026-04-22T20:53:27.589" CurrentBalance="0" seq_num="8" CurrentStatus="Opened" NumberOfLines="3" NumberOfOpenLines="2" NumberOfSettledLines="1" NumberOfLostLines="0" NumberOfWonLines="1" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="1705283464" EventName="Bournemouth vs Leeds" EventTypeName="Live Betting O/U" LeagueName="England - Premier League" YourBet="Under 3.5" HomeTeam="Bournemouth" AwayTeam="Leeds" DecimalOdds="1.41" UserOddStyle="Decimal" OddsInUserStyle="1.41" BranchID="1" BranchName="Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="5517581541" MasterLeagueID="24" Score="2:1" EventTypeID="39" EventDateUTC="2026-04-22 19:00" MasterEventID="1428514984" EventID="1428514984" NewMasterEventID="830846952735604736" NewEventID="830846952735604736" NewLeagueID="740557258543472640" NewLineID="0OU830846955327684687UMM" EncodedID="0OU830846955327684687UMM" EventState="" BestOddsApplied="0" SelectionID="1" DBOdds="1.41" IsLive="1" CurrentResult="" MarketID="0OU830846955327684687">
          <Changes>
            <Change ID="834893929160912947" OldStatus="{GetNewStatus(betAmount, creditAmount)}" NewStatus="Opened" Amount="{creditAmount}" PrevBalance="0" NewBalance="0" DateUTC="2026-04-22T20:53:27" TriggeredResult="">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="{creditAmount}" OldStatus="{GetNewStatus(betAmount, creditAmount)}" NewStatus="Opened" IsFreeBet="0" MaxPayout="3780853" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="2026-04-22T20:53:27" IsResettlement="1"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        await SendAsync("CreditCustomer (2 - Resettled)", url, xml);
    }

    private static async Task CallCredit3(Ids i)
    {
        var (username, betAmount, creditAmount, reserveId, agentId, customerId, purchaseId, betId, _, _, _, _, creditReqId3) = i;
        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={creditReqId3}&amount={creditAmount}&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="{creditAmount}" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="{creditAmount}" CreationDateUTC="2026-04-22T20:55:43.720" CurrentBalance="0" seq_num="10" CurrentStatus="Closed" NumberOfLines="3" NumberOfOpenLines="1" NumberOfSettledLines="2" NumberOfLostLines="1" NumberOfWonLines="1" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="1705283464" EventName="Bournemouth vs Leeds" EventTypeName="Live Betting O/U" LeagueName="England - Premier League" YourBet="Under 3.5" HomeTeam="Bournemouth" AwayTeam="Leeds" DecimalOdds="1.41" UserOddStyle="Decimal" OddsInUserStyle="1.41" BranchID="1" BranchName="Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="5517581541" MasterLeagueID="24" Score="2:1" EventTypeID="39" EventDateUTC="2026-04-22 19:00" MasterEventID="1428514984" EventID="1428514984" NewMasterEventID="830846952735604736" NewEventID="830846952735604736" NewLeagueID="740557258543472640" NewLineID="0OU830846955327684687UMM" EncodedID="0OU830846955327684687UMM" EventState="" BestOddsApplied="0" SelectionID="1" DBOdds="1.41" IsLive="1" CurrentResult="2 : 2" MarketID="0OU830846955327684687">
          <Changes>
            <Change ID="834895235875397678" OldStatus="Opened" NewStatus="{GetNewStatus(betAmount, creditAmount)}" Amount="{creditAmount}" PrevBalance="0" NewBalance="0" DateUTC="2026-04-22T20:55:43" TriggeredResult="2 : 2">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="{creditAmount}" OldStatus="Opened" NewStatus="{GetNewStatus(betAmount, creditAmount)}" IsFreeBet="0" MaxPayout="3780853" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="2026-04-22T20:55:43" IsResettlement="0"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        await SendAsync("CreditCustomer (3 - Final/Lost)", url, xml);
    }

    // ── MixParlay leg data ────────────────────────────────────────────────────
    private record MpLeg(
        string LineId, string EventName, string LeagueName, string YourBet,
        string HomeTeam, string AwayTeam, string DbOdds, string Score, string CurrentResult,
        string MasterLeagueId, string NewMasterEventId, string NewEventId, string NewLeagueId,
        string NewLineId, string MarketId, string SelectionId, string EventDateUtc,
        string MasterEventId, string LeagueId, string SettledDateUtc);

    private static MpLeg GetMpLeg(string leg) => leg switch
    {
        "1" => new MpLeg("2050955401", "Talleres [V] vs Colo-Colo [V]",
                   "V-Soccer Copa Libertadores - 12 mins [V]", "Under 6.5",
                   "Talleres [V]", "Colo-Colo [V]", "2.28", "2:2", "3 : 3",
                   "667610988841086976", "841887694421540864", "841887694421540864", "794449773779529728",
                   "0OU841887697361711133UMM", "0OU841887697361711133", "0",
                   "2026-05-12 07:26", "681992137", "5919112069", "2026-05-12T07:42:16"),
        "2" => new MpLeg("1249360371", "Vfb Stuttgart [V] vs FC Heidenheim 1846 [V]",
                   "V-Soccer Germany Bundesliga - 12 mins [V]", "Under 8.5",
                   "Vfb Stuttgart [V]", "FC Heidenheim 1846 [V]", "2.15", "4:1", "6 : 3",
                   "631408858384117760", "841888691298549760", "841888691298549760", "794691691839029248",
                   "0OU841888691910942749UMM", "0OU841888691910942749", "1",
                   "2026-05-12 07:28", "1530667357", "5920914499", "2026-05-12T07:44:46"),
        _   => new MpLeg("677552008", "Spain W [V] vs France W [V]",
                   "V-Soccer Women's Nations League - 12 mins [V]", "Under 7.5",
                   "Spain W [V]", "France W [V]", "1.77", "0:1", "2 : 6",
                   "634678815721041920", "841891187786002432", "841891187786002432", "794450022854078464",
                   "0OU841891188448763933UMM", "0OU841891188448763933", "2",
                   "2026-05-12 07:31", "1060820255", "5919113925", "2026-05-12T07:47:37"),
    };

    // ── MixParlay methods ─────────────────────────────────────────────────────
    private static async Task CallMpReserve(Ids i)
    {
        var (username, betAmount, _, reserveId, agentId, customerId, _, _, _, _, _, _, _) = i;
        string url = $"{BaseUrl}/Reserve?reserve_id={reserveId}&Amount={betAmount}&IsFreeBet=false&agent_id={agentId}&customer_id={customerId}&cust_id={username}";
        string xml = $"""
<Bets cust_id="{customerId}" player_level_id="0" reserve_id="{reserveId}" amount="{betAmount}" currency_code="THB" platform="W" bonus_amount="0.0000" real_amount="{betAmount}" ip_address="93.185.162.121" country="ID">
  <Bet BetID="0" BetTypeID="2" BetTypeName="Trebles X 1 bet" Stake="{betAmount}" OrgStake="{betAmount}" Gain="196.83" StakeTax="0" IsLive="1" NumberOfBets="1" Status="Opened" IsFreeBet="0" BonusID="0" FreebetAmount="0" RealAmount="{betAmount}" BonusAmount="0.0000" MaxPayout="3803082" CreationDate="2026-05-12T07:34:27" PurchaseBetID="0" Odds="796" OddsDec="8.9500" ComboBetNumersLines="3" ReferenceID="0" ReserveAmountType="Real" ReserveAmountTypeID="1" ContributionAmount="0.2200" ContributionType="0">
    <Lines LineID="2050955401" Stake="{betAmount}" OddsDec="2.2800" Gain="196.83" LiveScore1="2" LiveScore2="2" HomeTeam="Talleres [V]" AwayTeam="Colo-Colo [V]" Status="Opened" EventState="" CustomerID="{customerId}" BetID="0" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="234" BranchName="Virtual Soccer" LeagueID="551000818" LeagueName="V-Soccer Copa Libertadores - 12 mins [V]" MasterLeagueID="667610988841086976" CreationDate="2026-05-12T07:34:27" YourBet="Under 6.5" EventTypeID="6001" EventTypeName="Live Betting O/U" EventDate="2026-05-12T07:26:57" MasterEventID="681992137" EventID="681992137" NewMasterEventID="841887694421540864" NewEventID="841887694421540864" NewLeagueID="794449773779529728" NewLineID="0OU841887697361711133UMM" EventName="Talleres [V] vs Colo-Colo [V]" BetTypeID="2" Odds="128" DBOdds="2.28" Score="2:2" IsLive="1" Points="6.5" />
    <Lines LineID="1249360371" Stake="{betAmount}" OddsDec="2.2200" Gain="196.83" LiveScore1="4" LiveScore2="1" HomeTeam="Vfb Stuttgart [V]" AwayTeam="FC Heidenheim 1846 [V]" Status="Opened" EventState="" CustomerID="{customerId}" BetID="0" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="234" BranchName="Virtual Soccer" LeagueID="1378336405" LeagueName="V-Soccer Germany Bundesliga - 12 mins [V]" MasterLeagueID="631408858384117760" CreationDate="2026-05-12T07:34:27" YourBet="Under 8.5" EventTypeID="6001" EventTypeName="Live Betting O/U" EventDate="2026-05-12T07:28:57" MasterEventID="1530667357" EventID="1530667357" NewMasterEventID="841888691298549760" NewEventID="841888691298549760" NewLeagueID="794691691839029248" NewLineID="0OU841888691910942749UMM" EventName="Vfb Stuttgart [V] vs FC Heidenheim 1846 [V]" BetTypeID="2" Odds="122" DBOdds="2.22" Score="4:1" IsLive="1" Points="8.5" />
    <Lines LineID="677552008" Stake="{betAmount}" OddsDec="1.7700" Gain="196.83" LiveScore1="0" LiveScore2="1" HomeTeam="Spain W [V]" AwayTeam="France W [V]" Status="Opened" EventState="" CustomerID="{customerId}" BetID="0" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="234" BranchName="Virtual Soccer" LeagueID="7395355" LeagueName="V-Soccer Women's Nations League - 12 mins [V]" MasterLeagueID="634678815721041920" CreationDate="2026-05-12T07:34:27" YourBet="Under 7.5" EventTypeID="6001" EventTypeName="Live Betting O/U" EventDate="2026-05-12T07:31:57" MasterEventID="1060820255" EventID="1060820255" NewMasterEventID="841891187786002432" NewEventID="841891187786002432" NewLeagueID="794450022854078464" NewLineID="0OU841891188448763933UMM" EventName="Spain W [V] vs France W [V]" BetTypeID="2" Odds="-130" DBOdds="1.77" Score="0:1" IsLive="1" Points="7.5" />
  </Bet>
</Bets>
""";
        await SendAsync("MixParlay Reserve", url, xml, withAuth: true);
    }

    private static async Task CallMpDebitReserve(Ids i)
    {
        var (username, betAmount, _, reserveId, agentId, customerId, _, betId, _, purchaseBetId, _, _, _) = i;
        long reqId = Gen18();
        string url = $"{BaseUrl}/DebitReserve?req_id={reqId}&reserve_id={reserveId}&amount={betAmount}&IsFreeBet=false&AgentId={agentId}&CustomerId={customerId}&Username={username}&agent_id={agentId}&customer_id={customerId}&cust_id={username}";
        string xml = $"""
<Bets cust_id="{customerId}" reserve_id="{reserveId}" amount="{betAmount}" bonus_amount="0.0000" real_amount="{betAmount}" ip_address="93.185.162.121" country="ID" language_code="en">
  <Bet BetID="{betId}" BetTypeID="2" BetTypeName="Trebles X 1 bet" Stake="{betAmount}" OrgStake="{betAmount}" Gain="190.62" StakeTax="0" IsLive="1" NumberOfBets="1" Status="Opened" IsFreeBet="0" BonusID="0" FreebetAmount="0" RealAmount="{betAmount}" BonusAmount="0.0000" MaxPayout="3803082" CreationDate="2026-05-12T07:34:27" PurchaseBetID="{purchaseBetId}" Odds="768" OddsDec="8.6700" ComboBetNumersLines="3" UserOddStyle="Decimal" ReserveAmountType="Real" ReserveAmountTypeID="1" ContributionAmount="0.2200" ContributionType="0">
    <Lines LineID="2050955401" Stake="{betAmount}" OddsDec="2.2800" Gain="190.62" LiveScore1="2" LiveScore2="2" HomeTeam="Talleres [V]" AwayTeam="Colo-Colo [V]" Status="Opened" EventState="" CustomerID="{customerId}" BetID="{betId}" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="234" BranchName="Virtual Soccer" LeagueID="551000818" LeagueName="V-Soccer Copa Libertadores - 12 mins [V]" MasterLeagueID="667610988841086976" CreationDate="2026-05-12T07:34:27" YourBet="Under 6.5" EventTypeID="6001" EventTypeName="Live Betting O/U" EventDate="2026-05-12T07:26:57" MasterEventID="681992137" EventID="681992137" NewMasterEventID="841887694421540864" NewEventID="841887694421540864" NewLeagueID="794449773779529728" NewLineID="0OU841887697361711133UMM" EventName="Talleres [V] vs Colo-Colo [V]" BetTypeID="2" Odds="128" DBOdds="2.28" Score="2:2" IsLive="1" Points="6.5" />
    <Lines LineID="1249360371" Stake="{betAmount}" OddsDec="2.1500" Gain="190.62" LiveScore1="4" LiveScore2="1" HomeTeam="Vfb Stuttgart [V]" AwayTeam="FC Heidenheim 1846 [V]" Status="Opened" EventState="" CustomerID="{customerId}" BetID="{betId}" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="234" BranchName="Virtual Soccer" LeagueID="1378336405" LeagueName="V-Soccer Germany Bundesliga - 12 mins [V]" MasterLeagueID="631408858384117760" CreationDate="2026-05-12T07:34:27" YourBet="Under 8.5" EventTypeID="6001" EventTypeName="Live Betting O/U" EventDate="2026-05-12T07:28:57" MasterEventID="1530667357" EventID="1530667357" NewMasterEventID="841888691298549760" NewEventID="841888691298549760" NewLeagueID="794691691839029248" NewLineID="0OU841888691910942749UMM" EventName="Vfb Stuttgart [V] vs FC Heidenheim 1846 [V]" BetTypeID="2" Odds="115" DBOdds="2.15" Score="4:1" IsLive="1" Points="8.5" />
    <Lines LineID="677552008" Stake="{betAmount}" OddsDec="1.7700" Gain="190.62" LiveScore1="0" LiveScore2="1" HomeTeam="Spain W [V]" AwayTeam="France W [V]" Status="Opened" EventState="" CustomerID="{customerId}" BetID="{betId}" BetTypeName="Trebles X 1 bet" LineTypeID="3" LineTypeName="Live Betting O/U" BranchID="234" BranchName="Virtual Soccer" LeagueID="7395355" LeagueName="V-Soccer Women's Nations League - 12 mins [V]" MasterLeagueID="634678815721041920" CreationDate="2026-05-12T07:34:27" YourBet="Under 7.5" EventTypeID="6001" EventTypeName="Live Betting O/U" EventDate="2026-05-12T07:31:57" MasterEventID="1060820255" EventID="1060820255" NewMasterEventID="841891187786002432" NewEventID="841891187786002432" NewLeagueID="794450022854078464" NewLineID="0OU841891188448763933UMM" EventName="Spain W [V] vs France W [V]" BetTypeID="2" Odds="-130" DBOdds="1.77" Score="0:1" IsLive="1" Points="7.5" />
  </Bet>
</Bets>
""";
        Console.WriteLine($"  req_id : {reqId}");
        await SendAsync("MixParlay DebitReserve", url, xml, withAuth: true);
    }

    private static async Task CallMpCommitReserve(Ids i)
    {
        var (username, _, _, reserveId, agentId, customerId, _, _, _, purchaseBetId, _, _, _) = i;
        Console.WriteLine($"  reserve_id : {reserveId}");
        string url = $"{BaseUrl}/CommitReserve?cust_id={username}&reserve_id={reserveId}&agent_id={agentId}&customer_id={customerId}&purchase_id={purchaseBetId}";
        await SendAsync("MixParlay CommitReserve", url, method: HttpMethod.Get);
    }

    // Leg 1: Talleres [V] vs Colo-Colo [V] — always Won in this simulation
    private static async Task CallMpCreditLeg1(Ids i)
    {
        var (username, betAmount, _, reserveId, agentId, customerId, purchaseId, betId, _, _, creditReqId1, _, _) = i;
        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={creditReqId1}&amount=0&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="0" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="0" CreationDateUTC="2026-05-12T07:42:16.739" CurrentBalance="0" seq_num="24" CurrentStatus="Opened" NumberOfLines="3" NumberOfOpenLines="2" NumberOfSettledLines="1" NumberOfLostLines="0" NumberOfWonLines="1" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="2050955401" EventName="Talleres [V] vs Colo-Colo [V]" EventTypeName="Live Betting O/U" LeagueName="V-Soccer Copa Libertadores - 12 mins [V]" YourBet="Under 6.5" HomeTeam="Talleres [V]" AwayTeam="Colo-Colo [V]" DecimalOdds="2.28" UserOddStyle="Decimal" OddsInUserStyle="2.28" BranchID="234" BranchName="Virtual Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="5919112069" MasterLeagueID="667610988841086976" Score="2:2" EventTypeID="6001" EventDateUTC="2026-05-12 07:26" MasterEventID="681992137" EventID="681992137" NewMasterEventID="841887694421540864" NewEventID="841887694421540864" NewLeagueID="794449773779529728" NewLineID="0OU841887697361711133UMM" EncodedID="0OU841887697361711133UMM" EventState="" BestOddsApplied="0" SelectionID="0" DBOdds="2.28" IsLive="1" CurrentResult="3 : 3" MarketID="0OU841887697361711133">
          <Changes>
            <Change ID="841943097687793670" OldStatus="Opened" NewStatus="Won" Amount="0" PrevBalance="0" NewBalance="0" DateUTC="2026-05-12T07:42:16" TriggeredResult="3 : 3">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="0" OldStatus="Opened" NewStatus="Opened" IsFreeBet="0" MaxPayout="3803082" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="2026-05-12T07:42:16" IsResettlement="0"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        await SendAsync("MixParlay CreditCustomer (Leg 1 - Talleres/Colo-Colo Won)", url, xml);
    }

    // Leg 2: Vfb Stuttgart [V] vs FC Heidenheim 1846 [V] — Won/Lost based on overall result
    private static async Task CallMpCreditLeg2(Ids i)
    {
        var (username, betAmount, creditAmount, reserveId, agentId, customerId, purchaseId, betId, _, _, _, creditReqId2, _) = i;
        bool isWon = GetNewStatus(betAmount, creditAmount).ToString() == "Won";
        string leg2Status  = isWon ? "Won" : "Lost";
        string betNewStatus = isWon ? "Opened" : "Lost";
        string purchaseStatus = isWon ? "Opened" : "Closed";
        int lostLines = isWon ? 0 : 1;
        int wonLines  = isWon ? 2 : 1;
        int openLines = 1;
        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={creditReqId2}&amount=0&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="0" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="0" CreationDateUTC="2026-05-12T07:44:46.190" CurrentBalance="0" seq_num="28" CurrentStatus="{purchaseStatus}" NumberOfLines="3" NumberOfOpenLines="{openLines}" NumberOfSettledLines="2" NumberOfLostLines="{lostLines}" NumberOfWonLines="{wonLines}" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="1249360371" EventName="Vfb Stuttgart [V] vs FC Heidenheim 1846 [V]" EventTypeName="Live Betting O/U" LeagueName="V-Soccer Germany Bundesliga - 12 mins [V]" YourBet="Under 8.5" HomeTeam="Vfb Stuttgart [V]" AwayTeam="FC Heidenheim 1846 [V]" DecimalOdds="2.15" UserOddStyle="Decimal" OddsInUserStyle="2.15" BranchID="234" BranchName="Virtual Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="5920914499" MasterLeagueID="631408858384117760" Score="4:1" EventTypeID="6001" EventDateUTC="2026-05-12 07:28" MasterEventID="1530667357" EventID="1530667357" NewMasterEventID="841888691298549760" NewEventID="841888691298549760" NewLeagueID="794691691839029248" NewLineID="0OU841888691910942749UMM" EncodedID="0OU841888691910942749UMM" EventState="" BestOddsApplied="0" SelectionID="1" DBOdds="2.15" IsLive="1" CurrentResult="6 : 3" MarketID="0OU841888691910942749">
          <Changes>
            <Change ID="841943720898404358" OldStatus="Opened" NewStatus="{leg2Status}" Amount="0" PrevBalance="0" NewBalance="0" DateUTC="2026-05-12T07:44:46" TriggeredResult="6 : 3">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="0" OldStatus="Opened" NewStatus="{betNewStatus}" IsFreeBet="0" MaxPayout="3803082" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="2026-05-12T07:44:46" IsResettlement="0"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        await SendAsync($"MixParlay CreditCustomer (Leg 2 - Stuttgart/Heidenheim {leg2Status})", url, xml);
    }

    // Leg 3: Spain W [V] vs France W [V] — final settlement, amount sent only on full win
    private static async Task CallMpCreditLeg3(Ids i)
    {
        var (username, betAmount, creditAmount, reserveId, agentId, customerId, purchaseId, betId, _, _, _, _, creditReqId3) = i;
        bool isWon = GetNewStatus(betAmount, creditAmount).ToString() == "Won";
        string leg3Status      = isWon ? "Won"  : "Lost";
        string overallStatus   = isWon ? "Won"  : "Lost";
        string betOldStatus    = isWon ? "Opened" : "Lost";
        string creditXmlAmount = isWon ? creditAmount : "0";
        int lostLines = isWon ? 0 : 2;
        int wonLines  = isWon ? 3 : 1;
        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={creditReqId3}&amount={creditXmlAmount}&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="{creditXmlAmount}" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="{creditXmlAmount}" CreationDateUTC="2026-05-12T07:47:37.378" CurrentBalance="0" seq_num="38" CurrentStatus="Closed" NumberOfLines="3" NumberOfOpenLines="0" NumberOfSettledLines="3" NumberOfLostLines="{lostLines}" NumberOfWonLines="{wonLines}" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="677552008" EventName="Spain W [V] vs France W [V]" EventTypeName="Live Betting O/U" LeagueName="V-Soccer Women's Nations League - 12 mins [V]" YourBet="Under 7.5" HomeTeam="Spain W [V]" AwayTeam="France W [V]" DecimalOdds="1.77" UserOddStyle="Decimal" OddsInUserStyle="1.77" BranchID="234" BranchName="Virtual Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="5919113925" MasterLeagueID="634678815721041920" Score="0:1" EventTypeID="6001" EventDateUTC="2026-05-12 07:31" MasterEventID="1060820255" EventID="1060820255" NewMasterEventID="841891187786002432" NewEventID="841891187786002432" NewLeagueID="794450022854078464" NewLineID="0OU841891188448763933UMM" EncodedID="0OU841891188448763933UMM" EventState="" BestOddsApplied="0" SelectionID="2" DBOdds="1.77" IsLive="1" CurrentResult="2 : 6" MarketID="0OU841891188448763933">
          <Changes>
            <Change ID="841943434960093192" OldStatus="Opened" NewStatus="{leg3Status}" Amount="{creditXmlAmount}" PrevBalance="0" NewBalance="0" DateUTC="2026-05-12T07:47:37" TriggeredResult="2 : 6">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="{creditXmlAmount}" OldStatus="{betOldStatus}" NewStatus="{overallStatus}" IsFreeBet="0" MaxPayout="3803082" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="2026-05-12T07:47:37" IsResettlement="0"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        await SendAsync($"MixParlay CreditCustomer (Leg 3 - SpainW/FranceW {leg3Status} / Final: {overallStatus})", url, xml);
    }

    // ── MixParlay: rollback a settled leg back to Opened (IsResettlement=1) ──
    private static async Task CallMpRollbackLeg(Ids i)
    {
        var (username, betAmount, _, reserveId, agentId, customerId, purchaseId, betId, _, _, _, _, _) = i;
        Console.Write("  Leg to rollback (1/2/3) : ");
        string legInput = Console.ReadLine()?.Trim() ?? "1";
        Console.Write("  Current leg status (Won/Lost) : ");
        string oldStatus = Console.ReadLine()?.Trim() ?? "Won";

        var l = GetMpLeg(legInput);
        long reqId    = Gen18();
        long changeId = Gen18();

        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={reqId}&amount=0&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="0" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="0" CreationDateUTC="{l.SettledDateUtc}.000" CurrentBalance="0" seq_num="50" CurrentStatus="Opened" NumberOfLines="3" NumberOfOpenLines="1" NumberOfSettledLines="2" NumberOfLostLines="0" NumberOfWonLines="1" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="{l.LineId}" EventName="{l.EventName}" EventTypeName="Live Betting O/U" LeagueName="{l.LeagueName}" YourBet="{l.YourBet}" HomeTeam="{l.HomeTeam}" AwayTeam="{l.AwayTeam}" DecimalOdds="{l.DbOdds}" UserOddStyle="Decimal" OddsInUserStyle="{l.DbOdds}" BranchID="234" BranchName="Virtual Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="{l.LeagueId}" MasterLeagueID="{l.MasterLeagueId}" Score="{l.Score}" EventTypeID="6001" EventDateUTC="{l.EventDateUtc}" MasterEventID="{l.MasterEventId}" EventID="{l.MasterEventId}" NewMasterEventID="{l.NewMasterEventId}" NewEventID="{l.NewEventId}" NewLeagueID="{l.NewLeagueId}" NewLineID="{l.NewLineId}" EncodedID="{l.NewLineId}" EventState="" BestOddsApplied="0" SelectionID="{l.SelectionId}" DBOdds="{l.DbOdds}" IsLive="1" CurrentResult="{l.CurrentResult}" MarketID="{l.MarketId}">
          <Changes>
            <Change ID="{changeId}" OldStatus="{oldStatus}" NewStatus="Opened" Amount="0" PrevBalance="0" NewBalance="0" DateUTC="{l.SettledDateUtc}" TriggeredResult="{l.CurrentResult}">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="0" OldStatus="{oldStatus}" NewStatus="Opened" IsFreeBet="0" MaxPayout="3803082" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="{l.SettledDateUtc}" IsResettlement="1"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        Console.WriteLine($"  req_id : {reqId}");
        await SendAsync($"MixParlay Rollback Leg {legInput} ({oldStatus} → Opened)", url, xml);
    }

    // ── MixParlay: resettle a leg to a chosen outcome ─────────────────────────
    private static async Task CallMpResettleLeg(Ids i)
    {
        var (username, betAmount, creditAmount, reserveId, agentId, customerId, purchaseId, betId, _, _, _, _, _) = i;
        Console.Write("  Leg to resettle (1/2/3) : ");
        string legInput = Console.ReadLine()?.Trim() ?? "1";
        Console.Write("  New status (Won/Lost) : ");
        string newStatus = Console.ReadLine()?.Trim() ?? "Won";
        Console.Write($"  Credit amount (0 for partial, or actual amount) [{creditAmount}] : ");
        string amtInput = Console.ReadLine()?.Trim() ?? "";
        string amount   = string.IsNullOrEmpty(amtInput) ? creditAmount : amtInput;

        var l = GetMpLeg(legInput);
        long reqId    = Gen18();
        long changeId = Gen18();

        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={reqId}&amount={amount}&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="{amount}" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="{amount}" CreationDateUTC="{l.SettledDateUtc}.000" CurrentBalance="0" seq_num="55" CurrentStatus="Closed" NumberOfLines="3" NumberOfOpenLines="0" NumberOfSettledLines="3" NumberOfLostLines="0" NumberOfWonLines="3" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="0" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="{l.LineId}" EventName="{l.EventName}" EventTypeName="Live Betting O/U" LeagueName="{l.LeagueName}" YourBet="{l.YourBet}" HomeTeam="{l.HomeTeam}" AwayTeam="{l.AwayTeam}" DecimalOdds="{l.DbOdds}" UserOddStyle="Decimal" OddsInUserStyle="{l.DbOdds}" BranchID="234" BranchName="Virtual Soccer" LineTypeID="3" LineTypeName="Live Betting O/U" LeagueID="{l.LeagueId}" MasterLeagueID="{l.MasterLeagueId}" Score="{l.Score}" EventTypeID="6001" EventDateUTC="{l.EventDateUtc}" MasterEventID="{l.MasterEventId}" EventID="{l.MasterEventId}" NewMasterEventID="{l.NewMasterEventId}" NewEventID="{l.NewEventId}" NewLeagueID="{l.NewLeagueId}" NewLineID="{l.NewLineId}" EncodedID="{l.NewLineId}" EventState="" BestOddsApplied="0" SelectionID="{l.SelectionId}" DBOdds="{l.DbOdds}" IsLive="1" CurrentResult="{l.CurrentResult}" MarketID="{l.MarketId}">
          <Changes>
            <Change ID="{changeId}" OldStatus="Opened" NewStatus="{newStatus}" Amount="{amount}" PrevBalance="0" NewBalance="0" DateUTC="{l.SettledDateUtc}" TriggeredResult="{l.CurrentResult}">
              <Bets>
                <Bet ID="{betId}" BetType="Trebles X 1 bet" BetTypeID="2" PrevBalance="0" NewBalance="0" Amount="{amount}" OldStatus="Opened" NewStatus="{newStatus}" IsFreeBet="0" MaxPayout="3803082" BonusID="0" Odds="0" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="0" IsLive="1" BetSettledDate="{l.SettledDateUtc}" IsResettlement="0"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        Console.WriteLine($"  req_id : {reqId}");
        await SendAsync($"MixParlay Resettle Leg {legInput} (Opened → {newStatus}, amount={amount})", url, xml);
    }

    // ── Cashout (single bet) ──────────────────────────────────────────────────
    private static async Task CallCashout(Ids i)
    {
        var (username, betAmount, _, reserveId, agentId, customerId, purchaseId, betId, _, _, _, _, _) = i;
        Console.Write($"  Cashout amount [{betAmount}] : ");
        string amtInput    = Console.ReadLine()?.Trim() ?? "";
        string cashoutAmt  = string.IsNullOrEmpty(amtInput) ? betAmount : amtInput;
        long   reqId       = Gen18();
        long   changeId    = Gen18();
        // cashout odds = cashoutAmt / betAmount (rounded to 2 dp)
        string cashoutOdds = decimal.TryParse(cashoutAmt, out decimal co) && decimal.TryParse(betAmount, out decimal ba) && ba != 0
            ? Math.Round(co / ba, 2).ToString("F2")
            : "1.00";

        string url = $"{BaseUrl}/CreditCustomer?cust_id={username}&req_id={reqId}&amount={cashoutAmt}&purchase_id={purchaseId}&agent_id={agentId}&customer_id={customerId}&username={username}&reserve_id={reserveId}";
        string xml = $"""
<Credit CustomerID="{customerId}" CurrencyCode="THB" CustomerName="{username}" MerchantCustomerCode="{username}" Amount="{cashoutAmt}" DomainID="{DomainId}">
  <Purchases>
    <Purchase ReserveID="{reserveId}" MerchantReserveID="{reserveId}" PurchaseID="{purchaseId}" Amount="{cashoutAmt}" CreationDateUTC="2026-05-12T09:43:24.505" CurrentBalance="{cashoutAmt}" seq_num="2" CurrentStatus="Closed" NumberOfLines="1" NumberOfOpenLines="0" NumberOfSettledLines="1" NumberOfLostLines="0" NumberOfWonLines="0" NumberOfCanceledLines="0" NumberOfDrawLines="0" NumberOfCashoutLines="1" ExtBonusContribution="0">
      <Selections>
        <Selection LineID="957639931" EventName="Box Hill United vs Brunswick Juventus" EventTypeName="Live Betting 1X2" LeagueName="Australia Cup Qualifying" YourBet="X" HomeTeam="Box Hill United" AwayTeam="Brunswick Juventus" TransEventName="Box Hill United vs Brunswick Juventus" TransEventTypeName="Live Betting 1X2" TransLeagueName="Australia Cup Qualifying" TransYourBet="X" TransHomeTeam="Box Hill United" TransAwayTeam="Brunswick Juventus" TransBranchName="Soccer" DecimalOdds="2.68" UserOddStyle="Decimal" OddsInUserStyle="2.68" BranchID="1" BranchName="Soccer" LineTypeID="1" LineTypeName="1X2" LeagueID="6140569397" MasterLeagueID="554571750105780224" Score="0:0" EventTypeID="39" EventDateUTC="2026-05-12 09:00" MasterEventID="407387271" EventID="407387271" NewMasterEventID="841708030226907136" NewEventID="841708030226907136" NewLeagueID="824173273159139328" NewLineID="0ML841708031128756258D" EncodedID="0ML841708031128756258D" EventState="" BestOddsApplied="0" SelectionID="0" DBOdds="2.68" IsLive="1" MarketID="0ML841708031128756258">
          <Changes>
            <Change ID="{changeId}" OldStatus="Opened" NewStatus="Cashout" Amount="{cashoutAmt}" PrevBalance="0" NewBalance="{cashoutAmt}" DateUTC="2026-05-12T09:43:24">
              <Bets>
                <Bet ID="{betId}" BetType="Single bets" BetTypeID="1" PrevBalance="0" NewBalance="{cashoutAmt}" Amount="{cashoutAmt}" OldStatus="Opened" NewStatus="Cashout" IsFreeBet="0" BonusID="0" Odds="{cashoutOdds}" TaxPercentage="0" TaxAmount="0.0000" TOTax="0.0000" TaxedStake="{betAmount}" AmountBeforeTax="{cashoutAmt}" IsLive="1" BetSettledDate="2026-05-12T09:43:24" IsResettlement="0"/>
              </Bets>
            </Change>
          </Changes>
        </Selection>
      </Selections>
    </Purchase>
  </Purchases>
</Credit>
""";
        Console.WriteLine($"  req_id : {reqId}  cashout_odds : {cashoutOdds}");
        await SendAsync($"Cashout (amount={cashoutAmt})", url, xml);
    }
}
