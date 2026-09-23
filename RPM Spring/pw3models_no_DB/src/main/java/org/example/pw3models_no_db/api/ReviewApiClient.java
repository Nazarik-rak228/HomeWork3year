package org.example.pw3models_no_db.api;

import org.example.pw3models_no_db.model.ReviewModel;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.stereotype.Component;
import org.springframework.util.MultiValueMap;
import org.springframework.web.client.RestTemplate;

import java.util.List;

@Component
public class ReviewApiClient extends BaseApiClient {

    public ReviewApiClient(RestTemplate restTemplate,
                           @Value("${api.base.url}") String apiBaseUrl) {
        super(restTemplate, apiBaseUrl + "/api/review");
    }

    public List<ReviewModel> findAll() {
        return getList("", new ParameterizedTypeReference<List<ReviewModel>>() {});
    }

    public ReviewModel findById(int id) {
        return getOne("/" + id, ReviewModel.class);
    }

    public List<ReviewModel> findByRating(int rating) {
        return findAll().stream()
                .filter(r -> r.getRating() == rating)
                .toList();
    }

    public ReviewModel addReview(ReviewModel review) {
        MultiValueMap<String, String> params = form();
        add(params, "outUserId", review.getOutUser() != null ? review.getOutUser().getId() : null);
        add(params, "inUserId", review.getInUser() != null ? review.getInUser().getId() : null);
        add(params, "rating", review.getRating());
        add(params, "comment", review.getComment());
        return postForm("", params, ReviewModel.class);
    }

    public void updateReview(ReviewModel review) {
        MultiValueMap<String, String> params = form();
        add(params, "id", review.getId());
        add(params, "outUserId", review.getOutUser() != null ? review.getOutUser().getId() : null);
        add(params, "inUserId", review.getInUser() != null ? review.getInUser().getId() : null);
        add(params, "rating", review.getRating());
        add(params, "comment", review.getComment());
        putForm("", params);
    }

    public void deleteReview(int id) {
        delete("/" + id);
    }
}