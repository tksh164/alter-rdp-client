--
-- Create table for storing application settings
--

CREATE TABLE app_settings (
    json JSON NOT NULL
);

INSERT INTO app_settings (json) VALUES (json('
    {
        "mainWindow": {
            "width": 1024,
            "height": 768
        },
        "defaultRdpPort": 3389
    }
'));
