package org.example.pw3models_no_db.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.web.client.RestTemplateBuilder;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;

import java.time.Duration;
import java.util.Map;


@Component
public class ApiAuth {

    private static final long TOKEN_TTL_MS = 20 * 60 * 1000;
    private static final long REFRESH_BEFORE_MS = 5 * 60 * 1000;

    private final String baseUrl;
    private final String login;
    private final String password;
    private final RestTemplate rest;

    private volatile String cachedToken;
    private volatile long cachedAt;

    public ApiAuth(RestTemplateBuilder builder,
                   @Value("${api.base.url}") String baseUrl,
                   @Value("${api.username}") String login,
                   @Value("${api.password}") String password) {
        this.baseUrl = baseUrl;
        this.login = login;
        this.password = password;
        this.rest = builder
                .setConnectTimeout(Duration.ofSeconds(5))
                .setReadTimeout(Duration.ofSeconds(5))
                .build();
    }

    public String token() {
        long now = System.currentTimeMillis();
        if (cachedToken == null || now - cachedAt > TOKEN_TTL_MS - REFRESH_BEFORE_MS) {
            refresh();
        }
        return cachedToken;
    }

    public void invalidate() {
        cachedToken = null;
    }

    private synchronized void refresh() {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<Map<String, String>> entity =
                new HttpEntity<>(Map.of("username", login, "password", password), headers);
        @SuppressWarnings("unchecked")
        Map<String, Object> resp = rest.postForObject(baseUrl + "/api/auth/login", entity, Map.class);
        cachedToken = resp != null ? (String) resp.get("token") : null;
        cachedAt = System.currentTimeMillis();
    }
}