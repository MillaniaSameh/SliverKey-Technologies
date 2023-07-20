module default {
    type Contact {
        required contact_id: int64; 
        required first_name: str; 
        required last_name: str; 
        required email: str; 
        required title: str; 
        required birth_date: str; 
        required marital_status: bool; 
        description: str;
    }
}
