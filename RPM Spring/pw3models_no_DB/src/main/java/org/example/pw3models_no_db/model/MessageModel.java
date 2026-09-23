package org.example.pw3models_no_db.model;

import jakarta.validation.constraints.NotBlank;

import java.time.LocalDateTime;

public class MessageModel {

    private int id;

    private UserModel sender;

    private UserModel receiver;

    @NotBlank(message = "Текст сообщения не может быть пустым")
    private String text;

    private LocalDateTime createdAt;

    public MessageModel(int id, UserModel sender, UserModel receiver, String text, LocalDateTime createdAt) {
        this.id = id;
        this.sender = sender;
        this.receiver = receiver;
        this.text = text;
        this.createdAt = createdAt;
    }

    public MessageModel() {

    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public UserModel getSender() {
        return sender;
    }

    public void setSender(UserModel sender) {
        this.sender = sender;
    }

    public UserModel getReceiver() {
        return receiver;
    }

    public void setReceiver(UserModel receiver) {
        this.receiver = receiver;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}