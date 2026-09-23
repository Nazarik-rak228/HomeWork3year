package org.example.pw3models_no_db.config;

import org.springframework.boot.web.client.RestTemplateBuilder;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpHeaders;
import org.springframework.http.client.ClientHttpRequestInterceptor;
import org.springframework.http.client.ClientHttpResponse;
import org.springframework.web.client.RestTemplate;

import java.time.Duration;

@Configuration
public class ApiClientConfig {

    private final ApiAuth apiAuth;

    public ApiClientConfig(ApiAuth apiAuth) {
        this.apiAuth = apiAuth;
    }

    @Bean
    public RestTemplate restTemplate(RestTemplateBuilder builder) {
        ClientHttpRequestInterceptor authInterceptor = (request, body, execution) -> {
            request.getHeaders().set(HttpHeaders.AUTHORIZATION, "Bearer " + apiAuth.token());
            ClientHttpResponse response = execution.execute(request, body);
            if (response.getStatusCode().value() == 401) {
                response.close();
                apiAuth.invalidate();
                request.getHeaders().set(HttpHeaders.AUTHORIZATION, "Bearer " + apiAuth.token());
                response = execution.execute(request, body);
            }
            return response;
        };
        return builder
                .setConnectTimeout(Duration.ofSeconds(5))
                .setReadTimeout(Duration.ofSeconds(5))
                .interceptors(authInterceptor)
                .build();
    }
}