-- Seed Roles
INSERT INTO roles (name, slug, description)
VALUES
('Admin', 'admin', 'System administrator'),
('User', 'user', 'Normal user')
ON CONFLICT (slug) DO NOTHING;

-- Seed Admin user (password: admin123)
INSERT INTO users (email, password, username, role_id, is_email_confirmed)
VALUES (
    'admin@gmail.com',
    '$2a$11$u8Q7aP1QH3oH1Wl9kGf3eO8hX2z2m3V9rH1uQ8v2lq9tqzKk2p1yK',
    'admin',
    (SELECT id FROM roles WHERE slug = 'admin'),
    TRUE
)
ON CONFLICT (email) DO NOTHING;

-- Seed Normal user (password: user123)
INSERT INTO users (email, password, username, role_id, is_email_confirmed)
VALUES (
    'user@gmail.com',
    '$2a$11$u8Q7aP1QH3oH1Wl9kGf3eO8hX2z2m3V9rH1uQ8v2lq9tqzKk2p1yK',
    'user',
    (SELECT id FROM roles WHERE slug = 'user'),
    TRUE
)
ON CONFLICT (email) DO NOTHING;
