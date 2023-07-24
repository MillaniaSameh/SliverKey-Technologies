module default {
    type Contact {
        required username: str; 
        required password: str; 
        required contact_role: str; 
        required first_name: str; 
        required last_name: str; 
        required email: str; 
        required title: str; 
        required description: str;
        required birth_date: str; 
        required marital_status: bool; 
    }
}
