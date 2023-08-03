module default {
    type Contact {
        required first_name: str; 
        required last_name: str; 
        required email: str; 
        required title: str; 
        required description: str;
        required birth_date: datetime; 
        required marital_status: bool; 
    }
}
