CREATE MIGRATION m1qn4a2k2sy6nrgjg5wpofm5pxgzweddf5ddnvbbsfsp6uoapp5a6q
    ONTO m1yoclnzvaugkgxyz7dly6g57tv7pbzhufbxebaa6y7wacm75o2joa
{
  ALTER TYPE default::Contact {
      ALTER PROPERTY birth_date {
          SET TYPE std::datetime USING (<std::datetime>.birth_date);
      };
  };
};
