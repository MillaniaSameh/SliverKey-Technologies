CREATE MIGRATION m1yoclnzvaugkgxyz7dly6g57tv7pbzhufbxebaa6y7wacm75o2joa
    ONTO initial
{
  CREATE TYPE default::Contact {
      CREATE REQUIRED PROPERTY birth_date: std::str;
      CREATE REQUIRED PROPERTY description: std::str;
      CREATE REQUIRED PROPERTY email: std::str;
      CREATE REQUIRED PROPERTY first_name: std::str;
      CREATE REQUIRED PROPERTY last_name: std::str;
      CREATE REQUIRED PROPERTY marital_status: std::bool;
      CREATE REQUIRED PROPERTY title: std::str;
  };
};
