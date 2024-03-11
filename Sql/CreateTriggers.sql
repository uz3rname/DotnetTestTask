CREATE OR REPLACE FUNCTION update_available_amount()
RETURNS TRIGGER AS
$$
BEGIN
    IF NEW."Amount" <= 0 THEN
        RAISE EXCEPTION 'Amount for item "%" should be greater than zero',
            (SELECT "Name" FROM "StoreItems" WHERE "Id" = NEW."StoreItemId");
    END IF;
    IF NEW."Amount" > (SELECT "Amount" FROM "StoreItems" WHERE "Id" = NEW."StoreItemId") THEN
        RAISE EXCEPTION 'Reserved amount exceeds available amount for item "%"',
            (SELECT "Name" FROM "StoreItems" WHERE "Id" = NEW."StoreItemId");
    END IF;
    UPDATE "StoreItems"
    SET "Amount" = "Amount" - NEW."Amount"
    WHERE "Id" = NEW."StoreItemId";

    RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER update_available_amount_trigger
BEFORE INSERT ON "InvoiceItems"
FOR EACH ROW
EXECUTE FUNCTION update_available_amount();

CREATE OR REPLACE FUNCTION restore_available_amount()
RETURNS TRIGGER AS
$$
BEGIN
    UPDATE "StoreItems"
    SET "Amount" = "Amount" + OLD."Amount"
    WHERE "Id" = OLD."StoreItemId";

    RETURN OLD;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER restore_available_amount_trigger
AFTER DELETE ON "InvoiceItems"
FOR EACH ROW
EXECUTE FUNCTION restore_available_amount();