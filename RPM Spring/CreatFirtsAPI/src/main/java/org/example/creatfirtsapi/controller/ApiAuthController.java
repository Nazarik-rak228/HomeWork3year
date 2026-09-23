package org.example.creatfirtsapi.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.example.creatfirtsapi.config.JwtUtil;
import org.example.creatfirtsapi.dto.UserDto;
import org.example.creatfirtsapi.repository.UserRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Map;
import java.util.Optional;

@RestController
@RequestMapping("/api/auth")
@Tag(name = "Авторизация (JWT)", description = "Вход в систему и получение JWT-токена")
public class ApiAuthController {

    private final UserRepository userRepository;
    private final JwtUtil jwtUtil;
    private final BCryptPasswordEncoder encoder = new BCryptPasswordEncoder();

    public ApiAuthController(UserRepository userRepository, JwtUtil jwtUtil) {
        this.userRepository = userRepository;
        this.jwtUtil = jwtUtil;
    }

    @PostMapping("/login")
    @Operation(
            summary = "Вход в систему",
            description = "Проверяет логин и пароль пользователя и возвращает JWT-токен. " +
                    "Токен вставляется через кнопку Authorize в Swagger или в заголовок " +
                    "Authorization: Bearer <токен>.",
            security = {}
    )
    public ResponseEntity<?> login(@RequestBody LoginRequest request) {
        Optional<UserDto> opt = userRepository.findFirstByUsername(request.getUsername());
        if (opt.isEmpty() || !encoder.matches(request.getPassword(), opt.get().getPassword())) {
            return ResponseEntity.status(401)
                    .body(Map.of("error", "Неверный логин или пароль"));
        }
        UserDto user = opt.get();
        String role = user.getRole() != null ? user.getRole().getRolName() : "USER";
        String token = jwtUtil.generateToken(user.getUsername(), role);
        return ResponseEntity.ok(Map.of(
                "token", token,
                "username", user.getUsername(),
                "role", role
        ));
    }

    public static class LoginRequest {
        private String username;
        private String password;

        public String getUsername() {
            return username;
        }

        public void setUsername(String username) {
            this.username = username;
        }

        public String getPassword() {
            return password;
        }

        public void setPassword(String password) {
            this.password = password;
        }
    }
}