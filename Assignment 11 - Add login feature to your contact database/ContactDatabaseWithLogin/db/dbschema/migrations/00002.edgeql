CREATE MIGRATION m1lfoatxywumefhwhtpxladakqjptijzbiomhjkjxg3am3yespmvfa
    ONTO m1gx7egk3cr6mm6wyuyheuk3jtmlcrl3meprmvmrgne6sh3wtadg2q
{
  ALTER TYPE default::Contact {
      CREATE REQUIRED PROPERTY contact_role: std::str {
          SET REQUIRED USING (<std::str>{});
      };
  };
};
