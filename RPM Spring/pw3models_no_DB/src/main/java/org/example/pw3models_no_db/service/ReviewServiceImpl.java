package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.api.ReviewApiClient;
import org.example.pw3models_no_db.model.ReviewModel;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class ReviewServiceImpl implements ReviewService {

    private final ReviewApiClient reviewApiClient;

    public ReviewServiceImpl(ReviewApiClient reviewApiClient) {
        this.reviewApiClient = reviewApiClient;
    }

    @Override
    public List<ReviewModel> findAll() {
        return reviewApiClient.findAll();
    }

    @Override
    public ReviewModel addReview(ReviewModel review) {
        return reviewApiClient.addReview(review);
    }

    @Override
    public ReviewModel updateReview(ReviewModel review) {
        reviewApiClient.updateReview(review);
        return review;
    }

    @Override
    public void deleteReview(int id) {
        reviewApiClient.deleteReview(id);
    }

    @Override
    public List<ReviewModel> findByRating(int rating) {
        return reviewApiClient.findByRating(rating);
    }

    @Override
    public ReviewModel findById(int id) {
        return reviewApiClient.findById(id);
    }
}