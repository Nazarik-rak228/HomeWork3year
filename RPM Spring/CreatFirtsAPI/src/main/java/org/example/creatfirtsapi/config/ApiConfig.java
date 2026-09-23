package org.example.creatfirtsapi.config;

import io.swagger.v3.oas.annotations.OpenAPIDefinition;
import io.swagger.v3.oas.annotations.enums.SecuritySchemeType;
import io.swagger.v3.oas.annotations.info.Contact;
import io.swagger.v3.oas.annotations.info.Info;
import io.swagger.v3.oas.annotations.security.SecurityRequirement;
import io.swagger.v3.oas.annotations.security.SecurityScheme;

// Глобально все методы защищены JWT: выполните POST /api/auth/login,
// нажмите Authorize в Swagger и вставьте полученный токен.
@OpenAPIDefinition(
        info = @Info(
                title = "My First REST API",
                description = "API для фриланс-сайта TaskHub. " +
                        "Для вызова защищённых методов нужен JWT-токен: " +
                        "выполните POST /api/auth/login, затем нажмите кнопку Authorize и вставьте токен.",
                version = "1.0.0",
                contact = @Contact(
                        name = "Брагин Назар",
                        email = "Braginnazar228@gmail.com"
                )
        ),
        security = @SecurityRequirement(name = "bearerAuth")
)
@SecurityScheme(
        name = "bearerAuth",
        type = SecuritySchemeType.HTTP,
        scheme = "bearer",
        bearerFormat = "JWT"
)
public class ApiConfig {

}