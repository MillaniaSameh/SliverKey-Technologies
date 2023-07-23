CREATE MIGRATION m1o3kke7mxv7glvg6jqjseyn5dw5a5ffbk3m2ljdml4n2vbjaxxksa
    ONTO m1xegpbcevqzallwshq5zxqiaa7rc4me6p47vksfujmaygh27qeh6a
{
  ALTER TYPE default::Contact {
      DROP PROPERTY contact_id;
  };
};
