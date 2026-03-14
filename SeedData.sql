-- Script para insertar datos de ejemplo en el sistema de control de gastos

-- Insertar transacciones de ejemplo para el usuario admin (ID = 1)
-- Ingresos
INSERT INTO "Transactions" ("Description", "Amount", "Type", "Date", "Notes", "Location", "PaymentMethod", "IsRecurring", "RecurringFrequency", "NextRecurringDate", "CreatedAt", "UpdatedAt", "UserId", "CategoryId")
VALUES 
-- Ingresos de enero 2024
('Salario Enero 2024', 3500.00, 1, '2024-01-01', 'Salario mensual', 'Oficina', 'Transferencia bancaria', true, 'Monthly', '2024-02-01', NOW(), NOW(), 1, 1),
('Proyecto freelance - Sitio web', 1200.00, 1, '2024-01-15', 'Desarrollo de sitio web para cliente', 'Casa', 'PayPal', false, null, null, NOW(), NOW(), 1, 2),
('Dividendos de inversión', 150.00, 1, '2024-01-20', 'Dividendos trimestrales', 'Casa', 'Transferencia bancaria', false, null, null, NOW(), NOW(), 1, 3),

-- Gastos de enero 2024
('Supermercado semanal', 85.50, 2, '2024-01-05', 'Compra de alimentos', 'Supermercado Central', 'Tarjeta de débito', false, null, null, NOW(), NOW(), 1, 4),
('Gasolina', 45.00, 2, '2024-01-08', 'Llenado de tanque', 'Estación Shell', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 5),
('Alquiler de apartamento', 1200.00, 2, '2024-01-01', 'Alquiler mensual', 'Apartamento', 'Transferencia bancaria', true, 'Monthly', '2024-02-01', NOW(), NOW(), 1, 6),
('Cena en restaurante', 65.00, 2, '2024-01-12', 'Cena de aniversario', 'Restaurante El Buen Sabor', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 4),
('Netflix', 15.99, 2, '2024-01-15', 'Suscripción mensual', 'Online', 'Tarjeta de crédito', true, 'Monthly', '2024-02-15', NOW(), NOW(), 1, 8),
('Medicamentos', 35.00, 2, '2024-01-18', 'Medicina para resfriado', 'Farmacia San José', 'Efectivo', false, null, null, NOW(), NOW(), 1, 7),
('Ropa de invierno', 120.00, 2, '2024-01-22', 'Abrigo y guantes', 'Tienda de Ropa', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 9),

-- Ingresos de febrero 2024
('Salario Febrero 2024', 3500.00, 1, '2024-02-01', 'Salario mensual', 'Oficina', 'Transferencia bancaria', true, 'Monthly', '2024-03-01', NOW(), NOW(), 1, 1),
('Consultoría técnica', 800.00, 1, '2024-02-10', 'Asesoría en proyecto', 'Casa', 'Transferencia bancaria', false, null, null, NOW(), NOW(), 1, 2),

-- Gastos de febrero 2024
('Supermercado', 92.30, 2, '2024-02-03', 'Compra de alimentos', 'Supermercado Central', 'Tarjeta de débito', false, null, null, NOW(), NOW(), 1, 4),
('Gasolina', 48.00, 2, '2024-02-07', 'Llenado de tanque', 'Estación Shell', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 5),
('Alquiler de apartamento', 1200.00, 2, '2024-02-01', 'Alquiler mensual', 'Apartamento', 'Transferencia bancaria', true, 'Monthly', '2024-03-01', NOW(), NOW(), 1, 6),
('Luz y agua', 85.00, 2, '2024-02-05', 'Servicios públicos', 'Casa', 'Transferencia bancaria', false, null, null, NOW(), NOW(), 1, 6),
('Cine', 25.00, 2, '2024-02-14', 'Película de San Valentín', 'Cine Plaza', 'Efectivo', false, null, null, NOW(), NOW(), 1, 8),
('Netflix', 15.99, 2, '2024-02-15', 'Suscripción mensual', 'Online', 'Tarjeta de crédito', true, 'Monthly', '2024-03-15', NOW(), NOW(), 1, 8),
('Curso online', 199.00, 2, '2024-02-20', 'Curso de programación', 'Online', 'PayPal', false, null, null, NOW(), NOW(), 1, 10),

-- Ingresos de marzo 2024
('Salario Marzo 2024', 3500.00, 1, '2024-03-01', 'Salario mensual', 'Oficina', 'Transferencia bancaria', true, 'Monthly', '2024-04-01', NOW(), NOW(), 1, 1),
('Venta de proyecto', 2500.00, 1, '2024-03-15', 'Venta de aplicación móvil', 'Casa', 'Transferencia bancaria', false, null, null, NOW(), NOW(), 1, 2),
('Intereses de ahorros', 25.00, 1, '2024-03-31', 'Intereses mensuales', 'Banco', 'Transferencia bancaria', false, null, null, NOW(), NOW(), 1, 3),

-- Gastos de marzo 2024
('Supermercado', 78.90, 2, '2024-03-02', 'Compra de alimentos', 'Supermercado Central', 'Tarjeta de débito', false, null, null, NOW(), NOW(), 1, 4),
('Gasolina', 52.00, 2, '2024-03-09', 'Llenado de tanque', 'Estación Shell', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 5),
('Alquiler de apartamento', 1200.00, 2, '2024-03-01', 'Alquiler mensual', 'Apartamento', 'Transferencia bancaria', true, 'Monthly', '2024-04-01', NOW(), NOW(), 1, 6),
('Reparación de auto', 350.00, 2, '2024-03-12', 'Cambio de frenos', 'Taller Mecánico', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 5),
('Netflix', 15.99, 2, '2024-03-15', 'Suscripción mensual', 'Online', 'Tarjeta de crédito', true, 'Monthly', '2024-04-15', NOW(), NOW(), 1, 8),
('Libros técnicos', 89.00, 2, '2024-03-18', 'Libros de programación', 'Librería', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 10),
('Cena con amigos', 45.00, 2, '2024-03-25', 'Cena de cumpleaños', 'Restaurante La Terraza', 'Efectivo', false, null, null, NOW(), NOW(), 1, 4),

-- Datos del mes actual (septiembre 2024) para mostrar datos recientes
('Salario Septiembre 2024', 3500.00, 1, '2024-09-01', 'Salario mensual', 'Oficina', 'Transferencia bancaria', true, 'Monthly', '2024-10-01', NOW(), NOW(), 1, 1),
('Proyecto web - Cliente ABC', 1800.00, 1, '2024-09-10', 'Desarrollo de e-commerce', 'Casa', 'Transferencia bancaria', false, null, null, NOW(), NOW(), 1, 2),

-- Gastos recientes de septiembre 2024
('Supermercado', 95.50, 2, '2024-09-03', 'Compra de alimentos', 'Supermercado Central', 'Tarjeta de débito', false, null, null, NOW(), NOW(), 1, 4),
('Gasolina', 55.00, 2, '2024-09-05', 'Llenado de tanque', 'Estación Shell', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 5),
('Alquiler de apartamento', 1200.00, 2, '2024-09-01', 'Alquiler mensual', 'Apartamento', 'Transferencia bancaria', true, 'Monthly', '2024-10-01', NOW(), NOW(), 1, 6),
('Internet', 45.00, 2, '2024-09-02', 'Servicio de internet', 'Casa', 'Transferencia bancaria', false, null, null, NOW(), NOW(), 1, 6),
('Netflix', 15.99, 2, '2024-09-15', 'Suscripción mensual', 'Online', 'Tarjeta de crédito', true, 'Monthly', '2024-10-15', NOW(), NOW(), 1, 8),
('Spotify', 9.99, 2, '2024-09-15', 'Suscripción mensual', 'Online', 'Tarjeta de crédito', true, 'Monthly', '2024-10-15', NOW(), NOW(), 1, 8),
('Almuerzo de trabajo', 28.50, 2, '2024-09-20', 'Almuerzo con cliente', 'Restaurante del Centro', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 4),
('Gimnasio', 50.00, 2, '2024-09-01', 'Membresía mensual', 'Gimnasio Fitness', 'Transferencia bancaria', true, 'Monthly', '2024-10-01', NOW(), NOW(), 1, 7),
('Ropa de trabajo', 180.00, 2, '2024-09-18', 'Camisas y pantalones', 'Tienda de Ropa', 'Tarjeta de crédito', false, null, null, NOW(), NOW(), 1, 9),
('Curso de idiomas', 120.00, 2, '2024-09-22', 'Curso de inglés online', 'Online', 'PayPal', false, null, null, NOW(), NOW(), 1, 10);

