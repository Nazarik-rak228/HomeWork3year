package org.example.pw3models_no_db.model;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;

public class ReviewModel {

    private int id;

    private UserModel outUser;

    private UserModel inUser;

    @Min(value = 1, message = "Рейтинг от 1 до 5")
    @Max(value = 5, message = "Рейтинг от 1 до 5")
    private int rating;

    private String comment;

    public ReviewModel(int id, UserModel outUser, UserModel inUser, int rating, String comment) {
        this.id = id;
        this.outUser = outUser;
        this.inUser = inUser;
        this.rating = rating;
        this.comment = comment;
    }

    public ReviewModel() {

    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public UserModel getOutUser() {
        return outUser;
    }

    public void setOutUser(UserModel outUser) {
        this.outUser = outUser;
    }

    public UserModel getInUser() {
        return inUser;
    }

    public void setInUser(UserModel inUser) {
        this.inUser = inUser;
    }

    public int getRating() {
        return rating;
    }

    public void setRating(int rating) {
        this.rating = rating;
    }

    public String getComment() {
        return comment;
    }

    public void setComment(String comment) {
        this.comment = comment;
    }
}