using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactDatabase.Pages;

public class ContactsListModel : PageModel
{
    private readonly ILogger<ContactsListModel> _logger;
    public List<Contact> ContactsList { get; private set; } = new();

    public ContactsListModel(ILogger<ContactsListModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        ContactsList.Add(new Contact(1, "Harvey", "Specter", "specter@gmail.com", "Mr.", "Harvey Reginald Specter, J.D. is a former corporate attorney, one of the name partners at Specter Litt Wheeler Williams, the managing partner of Specter Litt, and a former Assistant District Attorney for the New York County District Attorney's Office. He is also the husband of Donna Paulsen.", "2000-06-22", false));
        ContactsList.Add(new Contact(2, "Luis", "Litt", "litt@gmail.com", "Mr.", "Louis Marlowe Litt, J.D., Esq. is a corporate attorney and the managing and name partner of Litt Wheeler Williams Bennett. He was promoted to the position of senior partner at Pearson Hardman by Daniel Hardman prior to the latter's second dismissal from the firm and was also the quartermaster of Pearson Darby.", "2000-06-22", false));
        ContactsList.Add(new Contact(3, "Donna", "Paulsen", "paulsen@gmail.com", "Ms.", "Donna Roberta Paulsen is the former Chief Operating Officer of Specter Litt Wheeler Williams and the wife of Harvey Specter, having originally worked as his legal secretary for over twelve years.", "2000-06-22", false));
        ContactsList.Add(new Contact(4, "Jessica", "Pearson", "pearson@gmail.com", "Ms.", "Jessica Lourdes Pearson, J.D. is the former managing partner of Pearson Specter Litt and current aide to the mayor of Chicago. She began to her legal career after being hired by Charles Van Dyke, one of the three name partners at Gordon Schmidt Van Dyke, due to the fact that she was black and female and the firm wished to fulfill a diversity quota. She made her way up the firm's ranks and, after becoming a senior partner, conspired with Daniel Hardman to oust the three partners and restructure the firm as Pearson Hardman, with Hardman taking over the reins as managing partner.", "2000-06-22", false));
        ContactsList.Add(new Contact(5, "Mike", "Ross", "ross@gmail.com", "Mr.", "Michael James is a former lawyer and junior partner at Specter Litt, a former legal consultant/supervisor at the Eastside Legal Clinic and a former investment banker at Sidwell Investment Group. He is the husband of Rachel Zane. He was hired by Harvey Specter, a senior partner at Pearson Hardman, as an associate lawyer, despite Mike not having graduated college or having a law degree. Mike left Pearson Specter after accepting the job offer to work at SIG; however, he returns later in season 4 after Jonathan Sidwell fired him from his firm. Shortly after his return, Louis Litt discovered his fraud and leveraged it to become name partner, forming Pearson Specter Litt. After Jack Soloff and the other partners at PSL acknowledge Mike's worth to the firm, he is promoted from junior associate to junior partner.", "2000-06-22", false));
        ContactsList.Add(new Contact(6, "Daniel", "Hardman", "hardman@gmail.com", "Mr.", "Daniel Hardman, J.D. was the co-founder of Pearson Hardman. He is an attorney and was the former managing partner before Jessica Pearson and Harvey Specter ousted him from the firm by threatening to expose his extramarital affair with an attorney at the firm to his wife Alicia, who was dying of cancer. Five years later, following his wife's death, Hardman returned to the firm to regain control and attempted to have Jessica and Harvey removed. His actions forced Jessica to merge with Edward Darby to end Hardman's plans, creating Pearson Darby in the process.", "2000-06-22", false));
        ContactsList.Add(new Contact(7, "Robert", "Zane", "zane@gmail.com", "Mr.", "Robert Lucas Zane, J.D. is Rachel Zane's father and a name partner at Zane Specter Litt Wheeler Williams, as well as the former managing and name partner at Rand, Kaldor & Zane and Zane Specter Litt and Samantha Wheeler's mentor.", "2000-06-22", true));
        ContactsList.Add(new Contact(8, "Samantha", "Wheeler", "wheeler@gmail.com", "Ms.", "Samantha Wheeler, J.D. is a corporate attorney and a name partner at Litt Wheeler Williams Bennett. Initially a senior partner at Rand, Kaldor & Zane, she left to follow Robert Zane at Zane Specter Litt. She is known to be extremely ruthless and hardworking, and was Robert's right hand, much like Harvey Specter was to Jessica Pearson.", "2000-06-22", false));
    }
}