package org.example.pw3models_no_db.api;

import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;
import org.springframework.web.client.RestTemplate;

import java.util.List;

public abstract class BaseApiClient {

    protected final RestTemplate restTemplate;
    protected final String baseUrl;

    protected BaseApiClient(RestTemplate restTemplate, String baseUrl) {
        this.restTemplate = restTemplate;
        this.baseUrl = baseUrl;
    }

    protected MultiValueMap<String, String> form() {
        return new LinkedMultiValueMap<>();
    }

    protected void add(MultiValueMap<String, String> map, String key, Object value) {
        if (value != null) {
            map.add(key, value.toString());
        }
    }

    protected <T> T postForm(String path, MultiValueMap<String, String> params, Class<T> type) {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_FORM_URLENCODED);
        HttpEntity<MultiValueMap<String, String>> entity = new HttpEntity<>(params, headers);
        return restTemplate.postForObject(baseUrl + path, entity, type);
    }

    protected void putForm(String path, MultiValueMap<String, String> params) {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_FORM_URLENCODED);
        HttpEntity<MultiValueMap<String, String>> entity = new HttpEntity<>(params, headers);
        restTemplate.put(baseUrl + path, entity);
    }

    protected <T> T postJson(String path, Object body, Class<T> type) {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<Object> entity = new HttpEntity<>(body, headers);
        return restTemplate.postForObject(baseUrl + path, entity, type);
    }

    protected void putJson(String path, Object body) {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<Object> entity = new HttpEntity<>(body, headers);
        restTemplate.put(baseUrl + path, entity);
    }

    protected <T> List<T> getList(String path, org.springframework.core.ParameterizedTypeReference<List<T>> ref) {
        org.springframework.http.ResponseEntity<List<T>> resp = restTemplate.exchange(
                baseUrl + path,
                org.springframework.http.HttpMethod.GET,
                null,
                ref
        );
        return resp.getBody() != null ? resp.getBody() : List.of();
    }

    protected <T> T getOne(String path, Class<T> type) {
        try {
            return restTemplate.getForObject(baseUrl + path, type);
        } catch (Exception e) {
            return null;
        }
    }

    protected void delete(String path) {
        restTemplate.delete(baseUrl + path);
    }
}