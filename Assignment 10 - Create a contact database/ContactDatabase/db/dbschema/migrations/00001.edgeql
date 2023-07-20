CREATE MIGRATION m1xegpbcevqzallwshq5zxqiaa7rc4me6p47vksfujmaygh27qeh6a
    ONTO initial
{
  CREATE TYPE default::Contact {
      CREATE REQUIRED PROPERTY birth_date: std::str;
      CREATE REQUIRED PROPERTY contact_id: std::int64;
      CREATE PROPERTY description: std::str;
      CREATE REQUIRED PROPERTY email: std::str;
      CREATE REQUIRED PROPERTY first_name: std::str;
      CREATE REQUIRED PROPERTY last_name: std::str;
      CREATE REQUIRED PROPERTY marital_status: std::bool;
      CREATE REQUIRED PROPERTY title: std::str;
  };
};
