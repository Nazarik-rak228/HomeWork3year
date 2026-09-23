package org.example.pw3models_no_db.controller;

import org.example.pw3models_no_db.model.ReviewModel;
import org.example.pw3models_no_db.model.UserModel;
import org.example.pw3models_no_db.service.ReviewService;
import org.example.pw3models_no_db.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.List;


@Controller
public class ReviewController {
    @Autowired
    private ReviewService reviewService;
    @Autowired
    private UserService userService;

    @GetMapping("/reviews/list")
    public String allReviews(Model model,
                             @RequestParam(defaultValue = "1") int page,
                             @RequestParam(required = false) Integer rating,
                             @RequestParam(required = false) Integer id) {

        List<ReviewModel> allReviews = reviewService.findAll();
        List<UserModel> users = userService.findAll();

        if (id != null) {
            ReviewModel found = reviewService.findById(id);
            allReviews = (found != null) ? List.of(found) : List.of();
        } else if (rating != null) {
            allReviews = reviewService.findByRating(rating);
        }

        int pageSize = 10;
        int totalItems = allReviews.size();
        int totalPages = (int) Math.ceil((double) totalItems / pageSize);
        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;
        int from = (page - 1) * pageSize;
        int to = Math.min(from + pageSize, totalItems);

        model.addAttribute("reviews", allReviews.subList(from, to));
        model.addAttribute("currentPage", page);
        model.addAttribute("totalPages", totalPages);
        model.addAttribute("searchRating", rating);
        model.addAttribute("searchId", id);
        model.addAttribute("users", users);
        return "reviewList";
    }

    @PostMapping("/reviews/add")
    public String addReview(@RequestParam int outUserId,
                            @RequestParam int inUserId,
                            @RequestParam int rating,
                            @RequestParam String comment) {

        UserModel outUser = userService.findById(outUserId);
        UserModel inUser = userService.findById(inUserId);

        if (outUser == null || inUser == null) return "redirect:/reviews/list";

        ReviewModel review = new ReviewModel(0, outUser, inUser, rating, comment);
        reviewService.addReview(review);
        return "redirect:/reviews/list";
    }

    @PostMapping("/reviews/update")
    public String updateReview(@RequestParam int id,
                               @RequestParam int outUserId,
                               @RequestParam int inUserId,
                               @RequestParam int rating,
                               @RequestParam String comment) {

        UserModel outUser = userService.findById(outUserId);
        UserModel inUser = userService.findById(inUserId);
        if (outUser == null || inUser == null) return "redirect:/reviews/list";

        ReviewModel review = new ReviewModel(id, outUser, inUser, rating, comment);
        reviewService.updateReview(review);
        return "redirect:/reviews/list";
    }

    @PostMapping("/reviews/delete")
    public String deleteReview(@RequestParam int id) {
        reviewService.deleteReview(id);
        return "redirect:/reviews/list";
    }
}