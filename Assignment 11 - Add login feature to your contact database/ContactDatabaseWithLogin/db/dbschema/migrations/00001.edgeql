CREATE MIGRATION m1gx7egk3cr6mm6wyuyheuk3jtmlcrl3meprmvmrgne6sh3wtadg2q
    ONTO initial
{
  CREATE TYPE default::Contact {
      CREATE REQUIRED PROPERTY birth_date: std::str;
      CREATE REQUIRED PROPERTY description: std::str;
      CREATE REQUIRED PROPERTY email: std::str;
      CREATE REQUIRED PROPERTY first_name: std::str;
      CREATE REQUIRED PROPERTY last_name: std::str;
      CREATE REQUIRED PROPERTY marital_status: std::bool;
      CREATE REQUIRED PROPERTY password: std::str;
      CREATE REQUIRED PROPERTY title: std::str;
      CREATE REQUIRED PROPERTY username: std::str;
  };
};
